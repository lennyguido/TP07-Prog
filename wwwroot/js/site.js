function validarRegistro()
{
    let nombreUsuario = document.getElementById("nombreUsuario").value;
    let contrasena = document.getElementById("contrasena").value;
    let nombre = document.getElementById("nombre").value;
    let apellido = document.getElementById("apellido").value;
    let tipoUsuario = document.getElementById("tipoUsuario").value;
    let mensajeError = document.getElementById("mensajeError");


    if (nombreUsuario.length < 4)
    {
        mensajeError.innerHTML = "-El nombre de usuario debe tener al menos 4 caracteres.";
        return false;
    }

    if (contrasena.length < 6)
    {
        mensajeError.innerHTML = "-La contraseña debe tener al menos 6 caracteres.";
        return false;
    }

    if (caracteresValidos(nombre) == false)
    {
        mensajeError.innerHTML = "-El nombre solo puede contener letras.";
        return false;
    }

    if (caracteresValidos(apellido) == false)
    {
        mensajeError.innerHTML = "-El apellido solo puede contener letras.";
        return false;
    }

    return true;
}

function caracteresValidos(texto)
{
    let caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúÁÉÍÓÚñÑ ";

    for (let i = 0; i < texto.length; i++)
    {
        if (caracteresPermitidos.includes(texto[i]) == false)
        {
            return false;
        }
    }

    return true;
}