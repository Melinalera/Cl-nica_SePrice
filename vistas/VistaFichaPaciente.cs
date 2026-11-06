using System.Data;
using System.Net.Mail;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    internal sealed class VistaFichaPaciente : Form
    {
        private readonly string dni;
        private bool fichaExistente;
        internal int IdPaciente { get; private set; }
        internal string NombreCompleto { get; private set; } = "";
        private readonly ComboBox fichaAnterior = new()
        {
            Location = new Point(185, 105), Width = 420,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly TextBox nombre = new() { Location = new Point(185, 155), Width = 420, MaxLength = 50 };
        private readonly TextBox apellido = new() { Location = new Point(185, 195), Width = 420, MaxLength = 50 };
        private readonly TextBox telefono = new() { Location = new Point(185, 235), Width = 420, MaxLength = 20 };
        private readonly TextBox email = new() { Location = new Point(185, 275), Width = 420, MaxLength = 100 };
        private readonly TextBox afiliado = new() { Location = new Point(185, 315), Width = 420, MaxLength = 50 };
        private readonly Label aviso = new()
        {
            Location = new Point(25, 365), Size = new Size(580, 70), ForeColor = Color.Firebrick
        };
        private readonly Button guardar = new()
        {
            Text = "Registrar paciente", Location = new Point(385, 450), Size = new Size(220, 30),
            Enabled = false
        };

        internal VistaFichaPaciente(string dni)
        {
            this.dni = dni;
            Text = "Ficha del paciente";
            ClientSize = new Size(640, 505);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold), UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            Controls.AddRange(new Control[]
            {
                volver,
                new Label
                {
                    Text = "Ficha del paciente", Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                    Location = new Point(110, 15), Size = new Size(420, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                },
                new Label { Text = $"DNI: {dni}", Location = new Point(25, 65), AutoSize = true },
                new Label { Text = "Ficha anterior sin DNI", Location = new Point(25, 108), AutoSize = true },
                fichaAnterior,
                new Label { Text = "Nombre", Location = new Point(25, 158), AutoSize = true }, nombre,
                new Label { Text = "Apellido", Location = new Point(25, 198), AutoSize = true }, apellido,
                new Label { Text = "Teléfono", Location = new Point(25, 238), AutoSize = true }, telefono,
                new Label { Text = "Email (opcional)", Location = new Point(25, 278), AutoSize = true }, email,
                new Label { Text = "Afiliado (opcional)", Location = new Point(25, 318), AutoSize = true }, afiliado,
                aviso, guardar
            });
            fichaAnterior.SelectedIndexChanged += (_, _) =>
            {
                if (fichaAnterior.SelectedItem is DataRowView ficha)
                    MostrarDatos(ficha.Row);
            };
            guardar.Click += (_, _) => Guardar();
            Load += (_, _) => CargarFicha();
        }

        private void MostrarDatos(DataRow ficha)
        {
            nombre.Text = ficha["nombre"].ToString();
            apellido.Text = ficha["apellido"].ToString();
            telefono.Text = ficha["telefono"].ToString();
            email.Text = ficha["email"].ToString();
            afiliado.Text = ficha["número_de_afiliado"].ToString();
        }

        private void CargarFicha()
        {
            guardar.Enabled = false;
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var consulta = new MySqlCommand("""
                    SELECT id_paciente, nombre, apellido, telefono, email, número_de_afiliado
                    FROM pacientes WHERE dni = @dni
                    """, conexion);
                consulta.Parameters.AddWithValue("@dni", dni);
                using var adaptador = new MySqlDataAdapter(consulta);
                using var encontrada = new DataTable();
                adaptador.Fill(encontrada);
                fichaExistente = encontrada.Rows.Count == 1;
                if (fichaExistente)
                {
                    DataRow ficha = encontrada.Rows[0];
                    IdPaciente = (int)ficha["id_paciente"];
                    MostrarDatos(ficha);
                    foreach (TextBox campo in new[] { nombre, apellido, telefono, email, afiliado })
                        campo.ReadOnly = true;
                    fichaAnterior.Enabled = false;
                    guardar.Text = "Usar paciente";
                    guardar.Enabled = true;
                    aviso.Text = "El paciente ya está registrado. Confirmá sus datos para continuar con la reserva.";
                    return;
                }

                using var anteriores = new MySqlDataAdapter("""
                    SELECT id_paciente, nombre, apellido, telefono, email, número_de_afiliado,
                           CONCAT(COALESCE(apellido, ''), ', ', COALESCE(nombre, ''),
                                  ' — Ficha N.º ', id_paciente) AS descripcion
                    FROM pacientes WHERE dni IS NULL OR TRIM(dni) = ''
                    ORDER BY apellido, nombre, id_paciente
                    """, conexion);
                var opciones = new DataTable();
                anteriores.Fill(opciones);
                DataRow nueva = opciones.NewRow();
                nueva["id_paciente"] = 0;
                nueva["descripcion"] = "Nuevo paciente";
                opciones.Rows.InsertAt(nueva, 0);
                fichaAnterior.DisplayMember = "descripcion";
                fichaAnterior.ValueMember = "id_paciente";
                fichaAnterior.DataSource = opciones;
                guardar.Enabled = true;
                aviso.Text = "Si el paciente ya tenía una ficha sin DNI, seleccionala arriba. Verificá su identidad antes de asociarla.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo consultar al paciente. Revisá la conexión y el script 08.";
            }
        }

        private void Guardar()
        {
            aviso.Text = "";
            if (fichaExistente)
            {
                NombreCompleto = $"{apellido.Text}, {nombre.Text}";
                DialogResult = DialogResult.OK;
                return;
            }
            if (string.IsNullOrWhiteSpace(nombre.Text) ||
                string.IsNullOrWhiteSpace(apellido.Text) ||
                string.IsNullOrWhiteSpace(telefono.Text))
            {
                aviso.Text = "Completá nombre, apellido y teléfono.";
                return;
            }
            if (email.Text.Trim().Length > 0 && !MailAddress.TryCreate(email.Text.Trim(), out _))
            {
                aviso.Text = "Revisá el email ingresado.";
                email.Focus();
                return;
            }
            if (fichaAnterior.SelectedItem is not DataRowView anterior)
                return;
            int idAnterior = (int)anterior["id_paciente"];
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();
                string sql = idAnterior == 0 ? """
                    INSERT INTO pacientes (dni, nombre, apellido, telefono, email, número_de_afiliado)
                    VALUES (@dni, @nombre, @apellido, @telefono, @email, @afiliado)
                    """ : """
                    UPDATE pacientes
                    SET dni = @dni, nombre = @nombre, apellido = @apellido,
                        telefono = @telefono, email = @email, número_de_afiliado = @afiliado
                    WHERE id_paciente = @id AND (dni IS NULL OR TRIM(dni) = '')
                    """;
                using var comando = new MySqlCommand(sql, conexion, transaccion);
                comando.Parameters.AddWithValue("@dni", dni);
                comando.Parameters.AddWithValue("@nombre", nombre.Text.Trim());
                comando.Parameters.AddWithValue("@apellido", apellido.Text.Trim());
                comando.Parameters.AddWithValue("@telefono", telefono.Text.Trim());
                comando.Parameters.AddWithValue("@email", email.Text.Trim().Length == 0 ? DBNull.Value : (object)email.Text.Trim());
                comando.Parameters.AddWithValue("@afiliado", afiliado.Text.Trim().Length == 0 ? DBNull.Value : (object)afiliado.Text.Trim());
                comando.Parameters.AddWithValue("@id", idAnterior);
                if (comando.ExecuteNonQuery() != 1)
                {
                    aviso.Text = "La ficha anterior ya fue actualizada. Cerrá y volvé a buscar al paciente.";
                    return;
                }
                IdPaciente = idAnterior == 0 ? checked((int)comando.LastInsertedId) : idAnterior;

                // Mantener los datos personales de una cuenta ya vinculada.
                using var cuenta = new MySqlCommand("""
                    UPDATE usuarios u JOIN pacientes p ON p.id_usuario = u.id_usuario
                    SET u.nombre = p.nombre, u.apellido = p.apellido, u.telefono = p.telefono,
                        u.email = COALESCE(p.email, u.email)
                    WHERE p.id_paciente = @id
                    """, conexion, transaccion);
                cuenta.Parameters.AddWithValue("@id", IdPaciente);
                cuenta.ExecuteNonQuery();
                transaccion.Commit();
                NombreCompleto = $"{apellido.Text.Trim()}, {nombre.Text.Trim()}";
                DialogResult = DialogResult.OK;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                CargarFicha();
                aviso.Text = "Ese DNI ya tiene una ficha. Revisá los datos que se cargaron.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el registro. Cerrá y buscá nuevamente el DNI antes de reintentar.";
            }
        }
    }
}
