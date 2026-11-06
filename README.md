# Clínica SePrice

Aplicación de escritorio en C# y Windows Forms para la gestión de consultorios externos, estudios e insumos.

## Requisitos

- Windows.
- SDK de .NET 10 y Visual Studio con las herramientas de desarrollo de escritorio de .NET.
- Servidor MySQL y MySQL Workbench.
- Una impresora configurada en Windows. Para guardar comprobantes en PDF se puede seleccionar Microsoft Print to PDF.

## Configuración de la conexión

Todos los formularios usan `ConexionBaseDatos.cs`. Configurar allí el servidor, el usuario y la contraseña de la instalación local.

El esquema utilizado por el proyecto es `clinica_seprice`, sin tilde. Es diferente de `clínica_seprice`.

La contraseña de MySQL corresponde a la conexión al servidor. La contraseña del usuario de la clínica corresponde al inicio de sesión de la aplicación.

## Instalación en una base nueva

1. En MySQL Workbench, crear el esquema.

   ```sql
   CREATE DATABASE IF NOT EXISTS clinica_seprice
       CHARACTER SET utf8mb4;
   ```

2. Comprobar que sea una instalación nueva, con el esquema vacío.
3. Ejecutar `mysql.sql` una sola vez para crear las tablas base y los usuarios de ejemplo.
4. Ejecutar los scripts adicionales en el orden indicado más abajo.
5. Abrir `Clínica_SePrice.csproj` en Visual Studio, restaurar las dependencias y compilar.
6. Ejecutar la aplicación e ingresar con una cuenta administrativa para acceder a los módulos de gestión.

**El archivo `mysql.sql` contiene instrucciones DROP TABLE que eliminan usuarios y perfiles. Se utiliza únicamente para la instalación inicial en un esquema vacío.**

## Actualización de una base existente

1. Respaldar la base desde MySQL Workbench antes de actualizarla.
2. Descargar los cambios del repositorio.
3. Ejecutar los scripts adicionales que todavía falten, siguiendo el orden numérico.
4. Revisar `ConexionBaseDatos.cs` y compilar.

Los scripts 01 a 07 crean tablas con `CREATE TABLE IF NOT EXISTS` y conservan los registros existentes. Si una tabla ya existe con otra estructura, esos scripts no modifican sus columnas ni corrigen diferencias de esquema.

El script 08 actualiza la estructura original de `pacientes`, conserva sus registros y permite fichas sin cuenta de acceso. Copia los datos personales de las cuentas ya vinculadas. Los DNI anteriores deben completarse verificando la identidad del paciente.

| Orden | Script | Tablas incorporadas |
|---|---|---|
| 1 | `sql/01_movimientos_insumos.sql` | Entradas y salidas por lote |
| 2 | `sql/02_acreditaciones_consultorios.sql` | Acreditaciones de pacientes |
| 3 | `sql/03_solicitudes_estudios_guardia.sql` | Solicitudes de guardia e internación |
| 4 | `sql/04_consumos_estudios_guardia.sql` | Consumos de insumos por solicitud |
| 5 | `sql/05_honorarios_consultorios.sql` | Atenciones, liquidaciones y detalle |
| 6 | `sql/06_compras_insumos.sql` | Compras vinculadas a movimientos de entrada |
| 7 | `sql/07_resultados_estudios_guardia.sql` | Resultados y registro de entrega |
| 8 | `sql/08_fichas_pacientes.sql` | Actualización de pacientes con ficha propia, DNI único y cuenta opcional |
| 9 | `sql/09_agendas_consultorios.sql` | Consultorios, agendas por profesional y asignación de horarios a reservas |
| 10 | `sql/10_integracion_turnos_master.sql` | Hora del turno y nuevos profesionales de ejemplo, conservando los perfiles existentes |

La instalación base debe incluir `usuarios`, `medicos`, `pacientes`, `administrativos`, `turnosConsultoriosMédicos` e `insumos`. Conservar los nombres exactos, incluido el acento de `Médicos`.

Para verificar las tablas instaladas en Workbench:

```sql
USE clinica_seprice;
SHOW TABLES;
```

## Recorridos disponibles para el administrativo

- **Insumos** permite crear lotes, registrar entradas, salidas y compras, consultar movimientos y buscar compras. Una compra ya genera la entrada y suma el stock.
- **Turnos → Acreditar paciente** busca turnos del día por DNI y registra identidad y modalidad de pago.
- **Turnos → Consultorios Externos** busca o registra la ficha del paciente por DNI y reserva un horario de una agenda, con profesional y consultorio. Permite un sobreturno por hora del profesional sobre horarios regulares ocupados, y genera un comprobante imprimible.
- **Turnos → Configurar agendas** crea consultorios y agendas por médico y fecha. Valida superposiciones y duraciones mínimas de 15 minutos, 25 para fisio-kinesiología y 30 para salud mental.
- **Turnos → Registrar atención** permite seleccionar un turno acreditado de hoy, indicar el profesional de la misma especialidad, la práctica y el honorario.
- **Estudios → Guardia / Internación** registra solicitudes sin turno.
- **Estudios → Solicitudes de guardia / internación** permite consultar por estado y prioridad, iniciar, cancelar y finalizar estudios. Al finalizar se registra el consumo por lote.
- **Cargar / Ver resultado** permite guardar el resultado del profesional, registrar su entrega y generar el comprobante de retiro.
- **Liquidación de honorarios** consulta las atenciones sin liquidar de un profesional y período, confirma la liquidación y permite consultar su detalle y comprobante.
- **Cerrar sesión** vuelve al login con los campos de ingreso vacíos.

Para guardar un comprobante como PDF, usar **Imprimir / Guardar PDF**, seleccionar **Microsoft Print to PDF** y elegir el nombre y la carpeta del archivo.

## Preparación de datos

Para probar acreditación y atención, ingresar el DNI en **Turnos → Consultorios Externos** y usar **Buscar / Registrar paciente**. Si ya existe, confirmar su ficha; si es nuevo, completar nombre, apellido y teléfono. Email y afiliado son opcionales. Una ficha anterior sin DNI se puede seleccionar para completar su identificación sin duplicarla.

Después guardar una reserva con fecha del día y especialidad, y buscar ese DNI en **Acreditar paciente**. Crear la ficha no genera una cuenta ni una contraseña de acceso.

Antes de reservar, configurar una agenda futura para el profesional. Los horarios disponibles se generan a partir de su duración y de las reservas existentes. Se comprueban nuevamente al guardar y se impiden reservas superpuestas para el paciente.

El script 10 agrega la columna `hora` si falta. Los turnos anteriores sin horario conocido conservan ese dato vacío. También agrega los nuevos profesionales de ejemplo de master mediante sus nombres de usuario, sin modificar especialidades ni identificadores existentes. Por ejemplo, un perfil de Ariel ya registrado como Pediatría conserva esa especialidad.

Las franjas horarias y los días de atención se obtienen de las agendas configuradas. El envío automático de correo queda pendiente de configuración e implementación. El código original de master tenía el envío SMTP comentado.

Un profesional necesita un registro en `usuarios` con rol `medico` y otro en `medicos`, con especialidad y matrícula. El registro de cuentas del login todavía no completa automáticamente las tablas de perfiles. Las nuevas fichas creadas desde reservas pueden tener `id_usuario` nulo.

Los honorarios se ingresan al registrar cada atención. El sistema todavía no tiene un tarifario automático.

## Trabajo en equipo

Un pull actualiza los archivos del proyecto. Las tablas nuevas deben incorporarse por separado ejecutando sus scripts en MySQL Workbench.

Los datos locales de pacientes, movimientos, solicitudes y liquidaciones permanecen en MySQL. No se transfieren al subir el código a GitHub.
