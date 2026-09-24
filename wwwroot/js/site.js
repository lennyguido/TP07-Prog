
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

document.addEventListener("DOMContentLoaded", () => {
    const feed = document.getElementById("feedPublicaciones");

    document.addEventListener("click", async (event) => {
        const likeButton = event.target.closest(".like-button");
        if (likeButton)
        {
            await alternarMeGusta(likeButton);
            return;
        }

        const btnVerMas = event.target.closest("#btnVerMas");
        if (btnVerMas)
        {
            await cargarMasPublicaciones(btnVerMas, feed);
        }
    });

    document.addEventListener("submit", async (event) => {
        const commentForm = event.target.closest(".comment-form");
        if (!commentForm)
        {
            return;
        }

        event.preventDefault();
        await enviarComentario(commentForm);
    });
});

async function alternarMeGusta(button)
{
    const idPublicacion = parseInt(button.dataset.postId, 10);

    const response = await fetch("/Home/ToggleMeGusta", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ idPublicacion })
    });

    if (!response.ok)
    {
        return;
    }

    const data = await response.json();
    const likesElement = document.getElementById(`likes-${idPublicacion}`);

    if (likesElement)
    {
        likesElement.textContent = data.cantidadMeGusta;
    }

    button.textContent = data.meGusta ? "Ya no me gusta" : "Me Gusta";
    button.classList.toggle("liked", data.meGusta);
}

async function enviarComentario(form)
{
    const idPublicacion = parseInt(form.dataset.postId, 10);
    const textarea = form.querySelector("textarea[name='texto']");
    const texto = textarea.value.trim();

    if (texto.length === 0)
    {
        return;
    }

    const response = await fetch("/Home/Comentar", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ idPublicacion, texto })
    });

    if (!response.ok)
    {
        return;
    }

    const data = await response.json();
    const commentsContainer = document.getElementById(`comments-${idPublicacion}`);
    const emptyState = commentsContainer.querySelector(".empty-state");

    if (emptyState)
    {
        emptyState.remove();
    }

    const comment = document.createElement("div");
    comment.className = "comment-item";
    comment.dataset.commentId = data.comentario.id;
    comment.innerHTML = `
        <strong>${escapeHtml(data.comentario.nombreUsuario)}</strong>
        <span>${data.comentario.fechaComentario}</span>
        <p>${escapeHtml(data.comentario.texto)}</p>
    `;

    commentsContainer.appendChild(comment);
    textarea.value = "";
}

async function cargarMasPublicaciones(button, container)
{
    const desde = parseInt(button.dataset.desde, 10);

    const response = await fetch(`/Home/ObtenerMas?desde=${desde}`);

    if (!response.ok)
    {
        return;
    }

    const html = await response.text();
    if (!html.trim())
    {
        button.remove();
        return;
    }

    container.insertAdjacentHTML("beforeend", html);

    const tieneMas = response.headers.get("X-Tiene-Mas") === "true";
    if (tieneMas)
    {
        button.dataset.desde = desde + 10;
    }
    else
    {
        button.remove();
    }
}

function escapeHtml(text)
{
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}