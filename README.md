# Gestor de Gastos y Presupuesto Personal Diario

## Información general

- **Estudiante:** Dilan Zapata
- **Tipo de aplicación:** Aplicación de escritorio para registrar ingresos y gastos personales.
- **Descripción:** Permite administrar movimientos financieros, consultar el total de ingresos y gastos y calcular el saldo resultante.
- **Necesidad que resuelve:** Facilita llevar un registro organizado de los movimientos financieros personales y conservarlos entre ejecuciones.
- **Objetivo:** Aplicar POO, Avalonia UI y ADO.NET con SQLite en una aplicación funcional con operaciones CRUD.

> El resumen muestra los totales de todos los movimientos guardados; no representa un límite de presupuesto diario configurable.

## Funcionalidades

- Registrar ingresos y gastos con concepto, categoría, fuente o medio de pago, fecha y monto.
- Consultar todos los movimientos y los totales de ingresos, gastos y saldo.
- Seleccionar un movimiento para cargarlo en el formulario y actualizarlo.
- Eliminar el movimiento seleccionado.
- Validar los campos obligatorios, el formato de fecha (`AAAA-MM-DD`) y que el monto sea mayor que cero.
- Guardar los movimientos localmente en SQLite para conservarlos después de cerrar la aplicación.
- Mostrar mensajes de confirmación y errores de validación o de base de datos en la interfaz.

## Tecnologías

- C# y .NET 10 (`net10.0`).
- Avalonia UI 12.1.5 (interfaz XAML).
- ADO.NET mediante `Microsoft.Data.Sqlite` 11.0.0.
- SQLite.

## Estructura del proyecto

```text
.
├── README.md
├── .gitignore
├── evidencias/                   # Capturas de las pruebas manuales
└── PresupuestoDiario/
    ├── App.axaml, App.axaml.cs   # Inicio y configuración de la aplicación Avalonia
    ├── MainWindow.axaml          # Interfaz gráfica
    ├── MainWindow.axaml.cs       # Eventos y enlace entre interfaz y servicio
    ├── Program.cs                # Punto de entrada
    ├── Models/
    │   ├── MovimientoFinanciero.cs # Modelo abstracto y validación encapsulada del monto
    │   ├── Gasto.cs              # Movimiento de gasto
    │   ├── Ingreso.cs            # Movimiento de ingreso
    │   ├── Data/
    │   │   ├── DatabaseManager.cs # Conexión e inicialización de SQLite
    │   │   └── MovimientoDAO.cs  # Consultas ADO.NET y operaciones CRUD
    │   └── Services/
    │       └── PresupuestoServicio.cs # Validaciones, reglas y cálculo de resumen
    └── PresupuestoDiario.csproj  # Framework y dependencias
```

### Organización y POO

- `MovimientoFinanciero` abstrae los datos y comportamientos comunes; `Ingreso` y `Gasto` heredan de esta clase.
- El monto está encapsulado y rechaza valores no positivos. El método `ImpactoEnSaldo` se implementa de forma polimórfica en los tipos de movimiento.
- `PresupuestoServicio` valida entradas y calcula el resumen, `MovimientoDAO` consulta SQLite, y `MainWindow` gestiona la interacción gráfica. Esta separación evita concentrar toda la lógica en la interfaz.

## Requisitos e instalación

1. Instala el **SDK de .NET 10**. Comprueba la instalación con `dotnet --version`.
2. Clona el repositorio y entra en la carpeta del proyecto:

   ```powershell
   git clone https://github.com/Omitar380/Parcial.git
   cd Parcial/PresupuestoDiario
   ```

3. Restaura los paquetes declarados en el archivo del proyecto:

   ```powershell
   dotnet restore
   ```

No es necesario instalar SQLite por separado: el proveedor `Microsoft.Data.Sqlite` está declarado como dependencia de NuGet. Se requiere conexión a internet para restaurar los paquetes la primera vez.

## Ejecución

Desde la carpeta que contiene `PresupuestoDiario.csproj`, ejecuta:

```powershell
dotnet run
```

También se puede compilar con `dotnet build`. La aplicación abre su ventana de escritorio Avalonia.

## Base de datos

La aplicación crea automáticamente la carpeta de datos y la tabla `Movimientos` al iniciar; no hay que ejecutar scripts manuales. La base se guarda en:

- **Windows:** `%LOCALAPPDATA%\PresupuestoDiario\presupuesto.db`
- **Linux:** `~/.local/share/PresupuestoDiario/presupuesto.db`
- **macOS:** `~/Library/Application Support/PresupuestoDiario/presupuesto.db`

La tabla almacena `Id` (clave primaria autoincremental), `Tipo`, `Concepto`, `Categoria`, `DetalleOrigen` (fuente del ingreso o medio de pago), `Fecha` y `Monto`.

`DatabaseManager` crea la conexión SQLite. `MovimientoDAO` utiliza `Microsoft.Data.Sqlite` y parámetros para ejecutar `INSERT`, `SELECT`, `UPDATE` y `DELETE`; `PresupuestoServicio` aplica las validaciones antes de enviar datos al DAO. Las conexiones, comandos y lectores se liberan con `using`.

## Evidencias de funcionamiento

Las capturas todavía deben tomarse ejecutando la aplicación en el equipo de entrega; no se incluyen imágenes simuladas. Guarda en `evidencias/` una captura real y legible de cada prueba y agrega aquí su descripción/enlace:

1. Registro y confirmación de un movimiento.
2. Consulta de movimientos y resumen.
3. Actualización de un movimiento.
4. Eliminación de un movimiento.
5. Validación con datos incorrectos y mensaje mostrado.
6. Persistencia: movimiento visible después de cerrar y volver a abrir la aplicación.

La carpeta `evidencias/README.md` contiene una lista de comprobación para completar esta sección.

## Problemas encontrados y soluciones

- **La base de datos estaba junto al ejecutable en `bin/`, una ubicación que puede cambiar al compilar o publicar y que no es adecuada para datos del usuario.** Se trasladó a una carpeta estable de datos de la aplicación dentro del perfil del usuario y se crea automáticamente al iniciar.
- **Una fecha con formato incorrecto podía guardarse como texto.** El servicio ahora valida que sea una fecha real con formato `AAAA-MM-DD` antes de guardar o actualizar.

## Pruebas manuales antes de entregar

Además de compilar con `dotnet build`, verifica las seis operaciones/escenarios de la sección de evidencias. No entregues la carpeta `bin/` ni `obj/`; están excluidas mediante `.gitignore`.
