async function GetServicesList(areaSelect,serviceSelect) {

    let select = document.getElementById(areaSelect);
    let areaId = select.options[select.selectedIndex].value;

    CleanList(serviceSelect);

    if (areaId != "") {
        try {
            let listaDaAggiornare = document.getElementById(serviceSelect);
            let url = new URL(window.location.origin + "/Administration/Finalities/APIGetServicesList");
            url.search = new URLSearchParams({
                AreaId: areaId
            });

            const response = await fetch(url,
                {
                    method: "GET",
                    headers: {
                        "Content-type": "application/json; charset=UTF-8"

                    },
                    credentials: "same-origin"

                }
            );
            if (!response.ok) {
                throw new Error("Errore di rete nella lettura lista");
            };
            const jsonData = await response.json();
            jsonData.forEach(elemento => {
                const option = document.createElement('option');
                option.value = elemento.Value;
                option.innerHTML = elemento.Text;
                listaDaAggiornare.appendChild(option);
            });

            console.log(jsonData);

        } catch (error) {
            console.error("Si è verificato un errore nella lettura della lista:", error);
        }
    }
}

function CleanList(listaId) {
    if (listaId != "") {
        let listaDaPulire = document.getElementById(listaId);
        listaDaPulire.innerHTML = '<option value="" selected>...</option>';
    }
}