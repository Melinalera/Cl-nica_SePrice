using System.Data;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaSolicitudesGuardia : Form
    {
        private readonly ComboBox cmbEstado = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(90, 91), Width = 180
        };
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 135), Size = new Size(960, 335),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 485), Size = new Size(960, 50),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ForeColor = Color.Firebrick
        };

        public VistaSolicitudesGuardia()
        {
            Text = "Solicitudes de guardia e internación";
            ClientSize = new Size(1000, 550);
            MinimumSize = Size;
            StartPosition = FormStartPosition.CenterParent;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            var titulo = new Label
            {
                Text = "Solicitudes de guardia e internación",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(12, 46), Size = new Size(976, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };
            cmbEstado.Items.AddRange(new object[]
            {
                "Pendiente", "En realización", "Realizado", "Cancelado", "Todos"
            });
            cmbEstado.SelectedIndex = 0;

            var actualizar = new Button
            {
                Text = "Actualizar", Location = new Point(285, 89), Size = new Size(100, 27)
            };
            actualizar.Click += (_, _) => CargarSolicitudes();
            var iniciar = new Button
            {
                Text = "Iniciar estudio", Location = new Point(420, 89), Size = new Size(130, 27)
            };
            iniciar.Click += (_, _) => CambiarEstado("En realización");
            var finalizar = new Button
            {
                Text = "Marcar realizado", Location = new Point(560, 89), Size = new Size(140, 27)
            };
            finalizar.Click += (_, _) => CambiarEstado("Realizado");
            var cancelar = new Button
            {
                Text = "Cancelar solicitud", Location = new Point(710, 89), Size = new Size(145, 27)
            };
            cancelar.Click += (_, _) => CambiarEstado("Cancelado");
            var resultado = new Button
            {
                Text = "Cargar / Ver resultado", Location = new Point(790, 12), Size = new Size(190, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            resultado.Click += (_, _) =>
            {
                aviso.Text = "";
                if (grilla.CurrentRow?.DataBoundItem is not DataRowView solicitud)
                {
                    aviso.Text = "Seleccioná una solicitud.";
                    return;
                }
                using var ventana = new VistaResultadoEstudioGuardia((int)solicitud["id_solicitud"]);
                ventana.ShowDialog(this);
            };
            Controls.AddRange(new Control[]
            {
                volver, titulo,
                new Label { Text = "Estado", Location = new Point(20, 94), AutoSize = true },
                cmbEstado, actualizar, iniciar, finalizar, cancelar, resultado, grilla, aviso
            });
            cmbEstado.SelectedIndexChanged += (_, _) => CargarSolicitudes();
            Load += (_, _) => CargarSolicitudes();
        }

        private bool CargarSolicitudes()
        {
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT id_solicitud, dni_paciente, nombre_paciente, estudio_solicitado,
                           origen, prioridad, estado, referencia_orden, fecha_solicitud
                    FROM solicitudes_estudios_guardia
                    WHERE (@estado = 'Todos' OR estado = @estado)
                    ORDER BY CASE WHEN prioridad = 'Urgente' THEN 0 ELSE 1 END,
                             fecha_solicitud, id_solicitud
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@estado", cmbEstado.SelectedItem!.ToString());
                using var adaptador = new MySqlDataAdapter(comando);
                var solicitudes = new DataTable();
                adaptador.Fill(solicitudes);
                grilla.DataSource = solicitudes;
                if (solicitudes.Rows.Count == 0)
                    aviso.Text = "No hay solicitudes para el estado seleccionado.";
                return true;
            }
            catch (MySqlException)
            {
                grilla.DataSource = null;
                aviso.Text = "No se pudieron cargar las solicitudes. Revisá la conexión y la tabla en MySQL.";
                return false;
            }
        }

        private void CambiarEstado(string nuevoEstado)
        {
            aviso.Text = "";
            if (grilla.CurrentRow?.DataBoundItem is not DataRowView solicitud)
            {
                aviso.Text = "Seleccioná una solicitud.";
                return;
            }

            // 1. Permitir solo los cambios previstos para una solicitud activa.
            string estadoActual = (string)solicitud["estado"];
            bool permitido = nuevoEstado switch
            {
                "En realización" => estadoActual == "Pendiente",
                "Realizado" => estadoActual == "En realización",
                "Cancelado" => estadoActual == "Pendiente" || estadoActual == "En realización",
                _ => false
            };
            if (!permitido)
            {
                aviso.Text = "Para iniciar debe estar pendiente; para finalizar, en realización. Las solicitudes cerradas conservan su estado.";
                return;
            }

            if (nuevoEstado == "Realizado")
            {
                using var consumo = new FrmConsumoEstudio(
                    (int)solicitud["id_solicitud"], (string)solicitud["estudio_solicitado"]);
                bool finalizado = consumo.ShowDialog(this) == DialogResult.OK;
                if (CargarSolicitudes() && finalizado)
                    aviso.Text = "Estudio finalizado. Consumo de insumos registrado.";
                return;
            }

            // 2. Actualizar únicamente si nadie cambió el estado desde la última consulta.
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                const string consulta = """
                    UPDATE solicitudes_estudios_guardia SET estado = @nuevo
                    WHERE id_solicitud = @id AND estado = @anterior
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@nuevo", nuevoEstado);
                comando.Parameters.AddWithValue("@id", solicitud["id_solicitud"]);
                comando.Parameters.AddWithValue("@anterior", estadoActual);
                int cambios = comando.ExecuteNonQuery();
                if (CargarSolicitudes())
                    aviso.Text = cambios == 1
                        ? $"Solicitud actualizada a {nuevoEstado}."
                        : "La solicitud cambió desde la última consulta. Revisá el listado actualizado.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el cambio. Presioná Actualizar antes de reintentar.";
            }
        }
    }
}
