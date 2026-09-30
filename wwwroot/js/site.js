function validarRegistro()
{
    document.getElementById("errorRegistro").innerHTML = "";

    const nombreUsuario = document.getElementById("nombreUsuario").value;
    const contrasena = document.getElementById("contrasena").value;
    const nombre = document.getElementById("nombre").value;
    const apellido = document.getElementById("apellido").value;
    const tipoUsuario = document.getElementById("tipoUsuario").value;

    if (nombreUsuario.trim().length < 4)
    {
        document.getElementById("errorRegistro").innerHTML = "El nombre de usuario debe tener al menos 4 caracteres.";
        return false;
    }

    if (contrasena.length < 6)
    {
        document.getElementById("errorRegistro").innerHTML = "La contraseña debe tener al menos 6 caracteres.";
        return false;
    }

    if (nombre.trim().length === 0 || caracteresValidos(nombre) == false)
    {
        document.getElementById("errorRegistro").innerHTML = "Completá el nombre usando solamente letras y espacios.";
        return false;
    }

    if (apellido.trim().length === 0 || caracteresValidos(apellido) == false)
    {
        document.getElementById("errorRegistro").innerHTML = "Completá el apellido usando solamente letras y espacios.";
        return false;
    }

    if (tipoUsuario === "")
    {
        document.getElementById("errorRegistro").innerHTML = "Seleccioná un tipo de usuario.";
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

function meGusta(idPublicacion)
{
    fetch('/Home/MeGusta?idPublicacion=' + idPublicacion, { method: 'GET' })
    .then(response => response.json())
    .then(data =>
    {
        if (data.id != 0)
        {
            document.getElementById("cantidadMeGusta" + idPublicacion).innerHTML = data.cantidadMeGusta;

            if (data.yaLeGusto == true)
            {
                document.getElementById("botonMeGusta" + idPublicacion).innerHTML = "Ya no me gusta";
            }
            else
            {
                document.getElementById("botonMeGusta" + idPublicacion).innerHTML = "Me Gusta";
            }
        }
    })
    .catch(error => console.log("Error:", error));
}

function comentar(idPublicacion)
{
    let texto = document.getElementById("textoComentario" + idPublicacion).value;

    if (texto.trim().length > 0)
    {
        fetch('/Home/Comentar?idPublicacion=' + idPublicacion + '&texto=' + texto, { method: 'POST' })
        .then(response => response.json())
        .then(data =>
        {
            if (data.idPublicacion != 0)
            {
                let comentarioNuevo = document.createElement("div");
                comentarioNuevo.innerHTML = "<div class='comentario'><p><strong>@" + data.nombreUsuario + "</strong> - " + data.fechaComentario + "</p><p>" + data.texto + "</p></div>";

                document.getElementById("comentarios" + idPublicacion).appendChild(comentarioNuevo);
                document.getElementById("textoComentario" + idPublicacion).value = "";
            }
        })
        .catch(error => console.log("Error:", error));
    }
}

let desde = 10;

function verMas()
{
    fetch('/Home/ObtenerMas?desde=' + desde, { method: 'GET' })
    .then(response => response.json())
    .then(data =>
    {
        for (let i = 0; i < data.length; i++)
        {
            mostrarPublicacion(data[i]);
        }

        desde = desde + 10;

        if (data.length < 10)
        {
            document.getElementById("botonVerMas").style.display = "none";
        }
    })
    .catch(error => console.log("Error:", error));
}

function mostrarPublicacion(publicacion)
{
    let usuarioLogueado = document.getElementById("estadoSesion").value;
    let contenido = "";
    contenido = contenido + "<h2>" + publicacion.titulo + "</h2>";
    contenido = contenido + "<p class='datos-publicacion'>@" + publicacion.nombreUsuario + " - " + publicacion.fechaPublicacion + "</p>";
    contenido = contenido + "<img src='/img/publicaciones/" + publicacion.imagen + "' alt='Imagen de la publicación'>";
    contenido = contenido + "<p>" + publicacion.descripcion + "</p>";
    contenido = contenido + "<div class='me-gusta'>";
    contenido = contenido + "<span>Me Gusta: </span><span id='cantidadMeGusta" + publicacion.id + "'>" + publicacion.cantidadMeGusta + "</span>";

    if (usuarioLogueado == "si")
    {
        if (publicacion.yaLeGusto == true)
        {
            contenido = contenido + " <button id='botonMeGusta" + publicacion.id + "' type='button' onclick='meGusta(" + publicacion.id + ")'>Ya no me gusta</button>";
        }
        else
        {
            contenido = contenido + " <button id='botonMeGusta" + publicacion.id + "' type='button' onclick='meGusta(" + publicacion.id + ")'>Me Gusta</button>";
        }
    }

    contenido = contenido + "</div>";
    contenido = contenido + "<div class='comentarios' id='comentarios" + publicacion.id + "'><h3>Comentarios</h3>";

    for (let i = 0; i < publicacion.comentarios.length; i++)
    {
        let comentario = publicacion.comentarios[i];
        contenido = contenido + "<div class='comentario'><p><strong>@" + comentario.nombreUsuario + "</strong> - " + comentario.fechaComentario + "</p><p>" + comentario.texto + "</p></div>";
    }

    contenido = contenido + "</div>";

    if (usuarioLogueado == "si")
    {
        contenido = contenido + "<div class='nuevo-comentario'>";
        contenido = contenido + "<textarea id='textoComentario" + publicacion.id + "' rows='2' placeholder='Escribí un comentario'></textarea>";
        contenido = contenido + "<button type='button' onclick='comentar(" + publicacion.id + ")'>Comentar</button>";
        contenido = contenido + "</div>";
    }

    document.getElementById("listaPublicaciones").innerHTML = document.getElementById("listaPublicaciones").innerHTML + "<article class='publicacion'>" + contenido + "</article>";
}
