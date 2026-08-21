
function validarRegistro()
{
    const nombreUsuario = document.getElementById("nombreUsuario").value;
    const contrasena = document.getElementById("contrasena").value;
    const nombre = document.getElementById("nombre").value;
    const apellido = document.getElementById("apellido").value;
    const tipoUsuario = document.getElementById("tipoUsuario").value;

    if (nombreUsuario.trim().length < 4)
    {
        return false;
    }

    if (contrasena.length < 6)
    {
        return false;
    }

    if (nombre.trim().length === 0 || caracteresValidos(nombre) == false)
    {
        return false;
    }

    if (apellido.trim().length === 0 || caracteresValidos(apellido) == false)
    {
        return false;
    }

    if (tipoUsuario === "")
    {
        return false;
    }

    return true;
}

function caracteresValidos(texto)
{
    const caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúÁÉÍÓÚñÑ ";

    for (let i = 0; i < texto.length; i++)
    {
        if (caracteresPermitidos.includes(texto[i]) == false)
        {
            return false;
        }
    }

    return true;
}