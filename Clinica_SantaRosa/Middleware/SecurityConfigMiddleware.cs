namespace ClinicaSR.PL.WebApp.Middleware
{
    public class SecurityConfigMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityConfigMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.ToLower() ?? "";
            string? usuarioID = context.Session.GetString("UsuarioID");
            string? usuarioRol = context.Session.GetString("UsuarioRol");

            // 2. Definir Rutas Públicas (Excepciones)
            if (path == "/" || path.Contains("/seguridad/login") || path.Contains("/css") || path.Contains("/js"))
            {
                await _next(context);
                return;
            }

            // 3. Protección contra "Saltos" por URL (Validar Sesión)
            if (string.IsNullOrEmpty(usuarioID))
            {
                context.Response.Redirect("/seguridad/login");
                return;
            }

            // 4. Gestión de Roles Centralizada
            // Ejemplo: Solo el Administrador ve la gestión de usuarios
            bool tieneAcceso = true;

            if (path.StartsWith("/administrador") && usuarioRol != "ADMINISTRADOR") tieneAcceso = false;
            else if (path.StartsWith("/recepcionista") && usuarioRol != "RECEPCIONISTA" && usuarioRol != "ADMINISTRADOR") tieneAcceso = false;
            else if (path.StartsWith("/cajero") && usuarioRol != "CAJERO" && usuarioRol != "ADMINISTRADOR") tieneAcceso = false;

            if (!tieneAcceso)
            {
                context.Response.Redirect("/seguridad/index");
                return;
            }

            await _next(context);
        }

    }
}
