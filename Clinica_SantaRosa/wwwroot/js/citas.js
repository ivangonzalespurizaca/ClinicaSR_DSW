// --- INICIALIZACIÓN ---
document.addEventListener("DOMContentLoaded", function () {
    // Escuchamos el cambio de fecha solo cuando el DOM esté listo
    const inputFecha = document.getElementById("fechaCita");
    if (inputFecha) {
        inputFecha.addEventListener("change", actualizarDisponibilidad);
    }

    // --- NUEVO: Cargar datos al abrir modales ---
    const modalPac = document.getElementById('modalPacientes');
    if (modalPac) {
        modalPac.addEventListener('shown.bs.modal', function () {
            document.getElementById("filtroPaciente").value = ""; // Limpiamos búsqueda previa
            ejecutarBusquedaPaciente(); // Listamos todo
        });
    }

    const modalMed = document.getElementById('modalMedicos');
    if (modalMed) {
        modalMed.addEventListener('shown.bs.modal', function () {
            document.getElementById("filtroMedico").value = ""; // Limpiamos búsqueda previa
            ejecutarBusquedaMedico(); // Listamos todo
        });
    }
});

// --- BÚSQUEDA DE PACIENTES ---
function ejecutarBusquedaPaciente() {
    let filtro = document.getElementById("filtroPaciente").value;

    fetch(`/Cita/BuscarPaciente?filtro=${filtro}`)
        .then(res => res.json())
        .then(data => {
            let html = data.map(p => `
                <tr>
                    <td>${p.dni}</td>
                    <td>${p.nombres} ${p.apellidos}</td>
                    <td><button class="btn btn-sm btn-success" onclick="seleccionarPaciente(${p.iD_Paciente}, '${p.nombres} ${p.apellidos}')">Elegir</button></td>
                </tr>`).join("");
            document.getElementById("tablaPacientes").innerHTML = html;
        });
}

function seleccionarPaciente(id, nombre) {
    document.getElementById("pacienteId").value = id;
    document.getElementById("pacienteNombre").value = nombre;
    // Cerramos el modal usando la instancia de Bootstrap
    const modal = bootstrap.Modal.getInstance(document.getElementById('modalPacientes'));
    if (modal) modal.hide();
}

// --- BÚSQUEDA DE MÉDICOS ---
function ejecutarBusquedaMedico() {
    let filtro = document.getElementById("filtroMedico").value;
    fetch(`/Cita/BuscarMedico?filtro=${filtro}`)
        .then(res => res.json())
        .then(data => {
            let html = data.map(m => `
                <tr>
                    <td>${m.nombres}</td>
                    <td>${m.especialidadBE.nombre}</td>
                    <td><button class="btn btn-sm btn-success" onclick="seleccionarMedico(${m.iD_Medico}, '${m.nombres}')">Elegir</button></td>
                </tr>`).join("");
            document.getElementById("tablaMedicos").innerHTML = html;
        });
}

function seleccionarMedico(id, nombre) {
    document.getElementById("medicoId").value = id;
    document.getElementById("medicoNombre").value = nombre;
    const modal = bootstrap.Modal.getInstance(document.getElementById('modalMedicos'));
    if (modal) modal.hide();

    cargarHorariosRegulares(id);

    actualizarDisponibilidad(); // Refrescamos si ya hay una fecha elegida
}

// --- GESTIÓN DE DISPONIBILIDAD ---
function actualizarDisponibilidad() {
    const inputMed = document.getElementById("medicoId");
    const inputFec = document.getElementById("fechaCita");
    const selectHora = document.getElementById("horaCita");

    // 1. Extraemos los valores y validamos que existan
    const idMed = inputMed ? inputMed.value : "";
    const fecha = inputFec ? inputFec.value : "";

    if (!idMed || !fecha || fecha === "0001-01-01") return;

    // 2. Lógica para obtener el día de la semana sin saltos (Local)
    const [anio, mes, dia] = fecha.split('-').map(Number);
    const fechaLocal = new Date(anio, mes - 1, dia);
    const diaSemana = fechaLocal.getDay(); // 0=Dom, 1=Lun, etc.

    // 3. Validación de días laborables
    if (diasTrabajoActual.length > 0 && !diasTrabajoActual.includes(diaSemana)) {
        Swal.fire("Médico no atiende", "El doctor no atiende el día seleccionado.", "info");
        selectHora.innerHTML = '<option value="">Día no laborable</option>';
        selectHora.disabled = true;
        return;
    }

    // 4. Petición al servidor (ahora con variables definidas)
    fetch(`/Cita/VerificarDisponibilidad?idMedico=${idMed}&fecha=${fecha}`)
        .then(res => {
            if (!res.ok) return res.json().then(e => { throw new Error(e.message); });
            return res.json();
        })
        .then(data => {
            selectHora.innerHTML = '<option value="">-- Seleccione Hora --</option>';
            selectHora.disabled = false;

            // Lógica de llenado de slots
            data.horariosConfigurados.forEach(horario => {
                let actual = parseTime(horario.horario_Entrada);
                let fin = parseTime(horario.horario_Salida);

                while (actual < fin) {
                    let timeStr = formatTime(actual);
                    const estaOcupada = data.horasOcupadas.some(h => h.startsWith(timeStr));

                    if (!estaOcupada) {
                        let opt = document.createElement("option");
                        opt.value = timeStr;
                        opt.text = timeStr;
                        selectHora.add(opt);
                    }
                    actual.setMinutes(actual.getMinutes() + 30);
                }
            });

            if (selectHora.options.length <= 1) {
                selectHora.innerHTML = '<option value="">No hay turnos disponibles</option>';
                selectHora.disabled = true;
            }
        })
        .catch(err => {
            console.error(err);
            Swal.fire("Error", "No se pudo verificar la disponibilidad.", "error");
        });
}

// --- REGISTRO FINAL ---
function confirmarRegistro() {
    // 1. Identificamos el botón para controlar su estado
    const btnRegistro = document.querySelector("button[onclick='confirmarRegistro()']");

    // Validaciones básicas antes de enviar
    const idPac = document.getElementById("pacienteId").value;
    const idMed = document.getElementById("medicoId").value;
    const fecha = document.getElementById("fechaCita").value;
    const hora = document.getElementById("horaCita").value;

    if (!idPac || !idMed || !fecha || !hora) {
        Swal.fire("Atención", "Por favor complete todos los campos obligatorios.", "warning");
        return;
    }

    // --- MEJORA: BLOQUEO DEL BOTÓN ---
    btnRegistro.disabled = true;
    btnRegistro.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Procesando...';

    const cita = {
        PacienteBE: { ID_Paciente: parseInt(idPac) },
        MedicoBE: { ID_Medico: parseInt(idMed) },
        Fecha_Cita: fecha,
        Hora_Cita: hora,
        Motivo: document.getElementById("motivo").value,
        ID_Usuario: 2
    };

    // --- MEJORA: MANEJO ROBUSTO DE FETCH ---
    fetch('/Cita/Registrar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cita)
    })
        .then(async res => {
            // Verificamos si la respuesta es exitosa (Status 200)
            const data = await res.json();
                if (!res.ok) {
                    // Si el servidor responde con un error (ej: 400 o 500), lanzamos una excepción
                    // con el mensaje que viene del C# (BC o SQL)
                    throw new Error(data.message || "No se pudo completar el registro.");
                }
            return data;
        })
        .then(idGenerado => {
            if (idGenerado > 0) {
                Swal.fire("Éxito", "Cita registrada con éxito.", "success")
                    .then(() => window.location.href = "/Cita/Listar");
            }
        })
        .catch(err => {
            // --- MEJORA: RE-HABILITAR PARA CONTINUAR EL FLUJO ---
            // Si hay un error, devolvemos el botón a su estado original
            btnRegistro.disabled = false;
            btnRegistro.innerHTML = "Registrar Cita";

            // Mostramos el error específico para que el usuario sepa qué corregir
            // Ejemplo: "El motivo es obligatorio" o "Médico ya tiene cita"
            Swal.fire("Error", err.message, "error");
        });
}

function cargarHorariosRegulares(idMed) {
    const contenedor = document.getElementById("contenedorHorariosRegulares");
    const lista = document.getElementById("listaHorariosRegulares");
    if (!contenedor || !lista) return;

    diasTrabajoActual = [];

    // Este mapa cubre todas las posibilidades: nombre en español, inglés o número
    const mapaIndices = {
        "DOMINGO": 0, "LUNES": 1, "MARTES": 2, "MIERCOLES": 3, "JUEVES": 4, "VIERNES": 5, "SABADO": 6,
        "0": 0, "1": 1, "2": 2, "3": 3, "4": 4, "5": 5, "6": 6
    };
    const etiquetas = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

    fetch(`/Cita/ListarHorarios?idMedico=${idMed}`)
        .then(res => res.json())
        .then(data => {
            let html = '<div class="list-group list-group-flush">';
            if (data && data.length > 0) {
                data.forEach(h => {
                    // Convertimos el valor a string y mayúsculas para buscar en el mapa
                    const valorOriginal = h.dia_Semana.toString().toUpperCase();
                    const indice = mapaIndices[valorOriginal];

                    if (indice !== undefined) {
                        diasTrabajoActual.push(indice);
                        html += `
                            <div class="list-group-item bg-light d-flex justify-content-between align-items-center py-1 px-2 border-bottom">
                                <span class="fw-bold text-primary">${etiquetas[indice]}:</span>
                                <span class="badge bg-secondary text-white">
                                    ${h.horario_Entrada.substring(0, 5)} - ${h.horario_Salida.substring(0, 5)}
                                </span>
                            </div>`;
                    }
                });
                lista.innerHTML = html + '</div>';
                contenedor.style.display = "block";
            } else {
                lista.innerHTML = '<div class="alert alert-warning p-2">Médico sin horarios configurados.</div>';
                contenedor.style.display = "block";
            }
        });
}

function verDetalleCita(id) {
    fetch(`/Cita/VerDetalle/${id}`)
        .then(res => res.text()) // Recibimos HTML (la Vista Parcial)
        .then(html => {
            document.getElementById("contenidoModalDetalle").innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('modalDetalleCita'));
            modal.show();
        })
        .catch(err => Swal.fire("Error", "No se pudo cargar el detalle", "error"));
}

// Helpers
function parseTime(t) { return new Date("1970-01-01T" + t); }
function formatTime(d) { return d.toTimeString().substring(0, 5); }