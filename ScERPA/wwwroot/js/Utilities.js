async function GetServicesList() {

    let select = document.getElementById("Input_AreaId");
    let areaId = select.options[select.selectedIndex].value;
    let userId = document.getElementById("Input_UserId").value;

    CleanList("Input_ServizioId");
    CleanList("Input_FinalitaId");

    if (areaId != "") {
        try {
            let listaDaAggiornare = document.getElementById("Input_ServizioId");
            let url = new URL(window.location.origin + "/Administration/UsersAdmin/APIGetServicesList");
            url.search = new URLSearchParams({
                UserId: userId,
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
    };
};
async function GetPurpousesList() {
    let selectArea = document.getElementById("Input_AreaId");
    let areaId = selectArea.options[selectArea.selectedIndex].value;
    let selectServizio = document.getElementById("Input_ServizioId");
    let serviceId = selectServizio.options[selectServizio.selectedIndex].value;
    let userId = document.getElementById("Input_UserId").value;
    CleanList("Input_FinalitaId");

    if (areaId != "" && serviceId != "") {
        try {
            let listaDaAggiornare = document.getElementById("Input_FinalitaId");
            let url = new URL(window.location.origin + "/Administration/UsersAdmin/APIGetFinalitiesList");
            url.search = new URLSearchParams({
                UserId: userId,
                AreaId: areaId,
                ServiceId: serviceId
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
    };
};

function CleanList(listaId) {
    if (listaId != "") {
        let listaDaPulire = document.getElementById(listaId);
        listaDaPulire.innerHTML = '<option value="" selected>...</option>';
    }
};