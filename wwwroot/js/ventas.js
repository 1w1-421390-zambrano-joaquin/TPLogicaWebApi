const $API_BASE = '/api';

const $selectElement = selector => document.querySelector(selector);
const $ventaCarrito = new Map();
const $formatMoneySimple = value => '$' + Number(value || 0).toFixed(2);

async function buscarYListarProductos(terminoBusqueda) {
  const $contenedor = $selectElement('#productosVentas');
  if (!$contenedor) return;

  const filtro = (terminoBusqueda || '').trim();
  if (!filtro) {
    $contenedor.innerHTML =
      '<div class="list-group-item">Debe ingresar un producto para buscar.</div>';
    return;
  }

  $contenedor.innerHTML = '<div class="list-group-item">Buscando...</div>';

  try {
    const response = await fetch(
      `${$API_BASE}/Productos/buscar?nombre=${encodeURIComponent(filtro)}`,
      { headers: authHeaders({ Accept: 'application/json' }) }
    );
    if (!response.ok) throw new Error('HTTP ' + response.status);

    const productos = await response.json();
    if (!productos || !productos.length) {
      $contenedor.innerHTML = '<div class="list-group-item">Sin resultados.</div>';
      return;
    }

    $contenedor.innerHTML = productos
      .map(producto => `
        <div class="list-group-item d-flex justify-content-between align-items-center">
          <div>
            <p class="mb-1 text-light">${producto.nombreComercial}</p>
            <small>Stock: ${producto.stock ?? '-'} | Precio: ${$formatMoneySimple(producto.precioUnitario)}</small>
          </div>
          <button class="btn btn-sm btn-success" data-add="${producto.idProducto}">
            <i class="fas fa-plus"></i>
          </button>
        </div>
      `)
      .join('');
  } catch (error) {
    console.error(error);
    $contenedor.innerHTML =
      '<div class="list-group-item text-danger">Error al buscar.</div>';
  }
}

function renderDetalleFactura() {
  const $cuerpoTabla = $selectElement('#detalleFacturaBody');
  if (!$cuerpoTabla) return;

  const filas = [];

  for (const [, item] of $ventaCarrito) {
    const subtotal = item.precioUnitario * item.cantidad;
    filas.push(`
      <tr data-id="${item.idProducto}">
        <td>${item.nombreComercial}</td>
        <td style="width: 90px;">
          <input type="number" min="1" class="form-control form-control-sm qty"
                 value="${item.cantidad}" data-id="${item.idProducto}">
        </td>
        <td>${$formatMoneySimple(item.precioUnitario)}</td>
        <td class="sub" data-id="${item.idProducto}">${$formatMoneySimple(subtotal)}</td>
        <td>
          <button class="btn btn-sm btn-danger" data-del="${item.idProducto}">
            <i class="fas fa-trash"></i>
          </button>
        </td>
      </tr>
    `);
  }

  $cuerpoTabla.innerHTML =
    filas.join('') ||
    '<tr><td colspan="5" class="text-center text-muted">Sin productos.</td></tr>';

  actualizarTotalFactura();
}

function actualizarTotalFactura() {
  const $totalElemento = $selectElement('#totalFactura');
  if (!$totalElemento) return;

  let total = 0;
  for (const [, item] of $ventaCarrito) {
    total += item.precioUnitario * item.cantidad;
  }
  $totalElemento.textContent = $formatMoneySimple(total);
}

document.addEventListener('DOMContentLoaded', () => {
  const $inputBuscar = $selectElement('#txtBuscar');
  const $botonBuscar = $selectElement('#btnBuscar');
  const $listaProductos = $selectElement('#productosVentas');

  if ($botonBuscar && $inputBuscar) {
    $botonBuscar.addEventListener('click', () => {
      buscarYListarProductos($inputBuscar.value);
    });
  }

  if ($inputBuscar) {
    $inputBuscar.addEventListener('input', () => {
      const filtro = $inputBuscar.value.trim();
      if (filtro.length >= 3) {
        buscarYListarProductos(filtro);
      } else if ($listaProductos) {
        $listaProductos.innerHTML =
          '<div class="list-group-item">Escriba al menos 3 letras para buscar un producto.</div>';
      }
    });

    $inputBuscar.addEventListener('keydown', event => {
      if (event.key === 'Enter') {
        event.preventDefault();
        const filtro = $inputBuscar.value.trim();
        if (filtro.length >= 3) {
          buscarYListarProductos(filtro);
        } else if ($listaProductos) {
          $listaProductos.innerHTML =
            '<div class="list-group-item">Escriba al menos 3 letras para buscar un producto.</div>';
        }
      }
    });
  }

  const $contenedorProductos = $selectElement('#productosVentas');
  if ($contenedorProductos) {
    $contenedorProductos.addEventListener('click', async event => {
      const $boton = event.target.closest('[data-add]');
      if (!$boton) return;

      const idProducto = Number($boton.dataset.add);
      try {
        const response = await fetch(`${$API_BASE}/Productos/${idProducto}`, {
          headers: authHeaders({ Accept: 'application/json' })
        });
        if (!response.ok) throw new Error('HTTP ' + response.status);

        const producto = await response.json();
        const existente = $ventaCarrito.get(idProducto);

        $ventaCarrito.set(idProducto, {
          idProducto,
          nombreComercial: producto.nombreComercial,
          precioUnitario: Number(producto.precioUnitario),
          cantidad: existente ? existente.cantidad + 1 : 1
        });

        renderDetalleFactura();
      } catch (error) {
        console.error(error);
        alert('No se pudo agregar el producto.');
      }
    });
  }

  const $cuerpoDetalle = $selectElement('#detalleFacturaBody');
  if ($cuerpoDetalle) {
    $cuerpoDetalle.addEventListener('input', event => {
      const $inputCantidad = event.target.closest('input.qty');
      if (!$inputCantidad) return;

      const idProducto = Number($inputCantidad.dataset.id);
      const item = $ventaCarrito.get(idProducto);
      if (!item) return;

      const nuevaCantidad = Math.max(1, Number($inputCantidad.value || 1));
      $inputCantidad.value = nuevaCantidad;
      item.cantidad = nuevaCantidad;

      const $celdaSubtotal = document.querySelector(
        `.sub[data-id="${idProducto}"]`
      );
      if ($celdaSubtotal) {
        $celdaSubtotal.textContent = $formatMoneySimple(
          item.precioUnitario * item.cantidad
        );
      }

      actualizarTotalFactura();
    });

    $cuerpoDetalle.addEventListener('click', event => {
      const $botonEliminar = event.target.closest('[data-del]');
      if (!$botonEliminar) return;

      const idProducto = Number($botonEliminar.dataset.del);
      $ventaCarrito.delete(idProducto);
      renderDetalleFactura();
    });
  }
});

(function setupClientePorDni() {
  const $apiUrl = $API_BASE;
  const $inputNombre = document.getElementById('nombreClienteInput');
  const $inputDni = document.getElementById('clienteDni');
  const $botonBuscar = document.getElementById('btnBuscarCliente');
  const $botonEditar = document.getElementById('btnEditarCliente');
  const $mensaje = document.getElementById('clienteMsg');
  const $inputIdHidden = document.getElementById('clienteIdHidden');

  if (!$inputNombre && !$inputDni) return;

  if ($inputNombre) $inputNombre.disabled = true;
  if ($botonEditar) $botonEditar.disabled = true;

  const setMensajeCliente = (texto, esError = true) => {
    if ($mensaje) {
      $mensaje.textContent = texto || '';
      $mensaje.classList.toggle('text-danger', !!texto && esError);
      $mensaje.classList.toggle('text-success', !!texto && !esError);
      $mensaje.classList.toggle('d-none', !texto);
    }
    if ($inputDni) {
      $inputDni.classList.toggle('is-invalid', !!texto && esError);
      $inputDni.classList.toggle('is-valid', !!texto && !esError);
    }
  };

  const bloquearUiCliente = cliente => {
    const nombreCompleto =
      cliente.nombreCompleto ??
      `${cliente.nomCliente ?? cliente.nombre ?? ''} ${cliente.apeCliente ?? cliente.apellido ?? ''}`.trim();

    if ($inputNombre) $inputNombre.value = nombreCompleto || '';
    if ($inputDni) {
      $inputDni.value = (cliente.dni ?? cliente.documento ?? cliente.cuit ?? '').toString();
      $inputDni.disabled = true;
    }
    if ($inputIdHidden) {
      $inputIdHidden.value = cliente.idCliente ?? cliente.id ?? '';
    }
    if ($botonBuscar) $botonBuscar.disabled = true;
    if ($botonEditar) $botonEditar.disabled = false;
  };

  const limpiarUiCliente = () => {
    if ($inputDni) {
      $inputDni.disabled = false;
      $inputDni.value = '';
    }
    if ($botonBuscar) $botonBuscar.disabled = false;
    if ($inputNombre) $inputNombre.value = '';
    if ($inputIdHidden) $inputIdHidden.value = '';
    setMensajeCliente('', true);
    if ($inputDni) $inputDni.focus();
    if (window.__clearClienteSelect) window.__clearClienteSelect();
  };

  const soloDigitos = texto => String(texto ?? '').replace(/\D+/g, '');

  async function buscarClientePorDni() {
    setMensajeCliente('');
    if ($inputDni?.disabled) {
      setMensajeCliente('Tocá el lápiz para editar el DNI', true);
      return;
    }

    const dni = soloDigitos($inputDni?.value || '');
    if (!dni) {
      setMensajeCliente('Ingrese DNI', true);
      return;
    }
    if (dni.length < 1 || dni.length > 11) {
      setMensajeCliente('DNI inválido', true);
      return;
    }

    const url = `${$apiUrl}/Clientes?dni=${encodeURIComponent(dni)}&_=${Date.now()}`;

    try {
      const response = await fetch(url, {
        headers: authHeaders({ Accept: 'application/json' }),
        cache: 'no-store'
      });

      if (response.status === 404) {
        setMensajeCliente('Cliente no encontrado', true);
        return;
      }
      if (!response.ok) throw new Error('HTTP ' + response.status);

      const data = await response.json();
      const docFrom = item =>
        soloDigitos(
          item?.dni ?? item?.documento ?? item?.cuit ?? item?.DNI ?? item?.CUIT
        );

      const cliente = Array.isArray(data)
        ? data.find(item => docFrom(item) === dni) || null
        : docFrom(data) === dni
        ? data
        : null;

      if (!cliente) {
        setMensajeCliente('Cliente no encontrado', true);
        return;
      }

      bloquearUiCliente(cliente);
      setMensajeCliente('Cliente seleccionado', false);
      if (window.__syncClienteSelect) window.__syncClienteSelect(cliente);
    } catch (error) {
      console.error(error);
      setMensajeCliente('Error al buscar', true);
    }
  }

  if ($botonBuscar) {
    $botonBuscar.addEventListener('click', buscarClientePorDni);
  }

  if ($inputDni) {
    $inputDni.addEventListener('keydown', event => {
      if (event.key === 'Enter') {
        event.preventDefault();
        buscarClientePorDni();
      }
    });

    $inputDni.addEventListener('input', () => {
      $inputDni.value = soloDigitos($inputDni.value);
    });
  }

  if ($botonEditar) {
    $botonEditar.addEventListener('click', limpiarUiCliente);
  }

  window.setMsg = window.setMsg || setMensajeCliente;
  window.lockUI = window.lockUI || bloquearUiCliente;
  window.unlockUI = window.unlockUI || limpiarUiCliente;
})();

(function setupClienteSelect() {
  const $apiUrl = $API_BASE;
  const $selectCliente = document.getElementById('clienteSelect');
  const $filtroCliente = document.getElementById('clienteSelectFilter');
  const $mensajeCliente = document.getElementById('clienteSelectMsg');

  if (!$selectCliente) return;

  const mostrarMensajeCliente = texto => {
    if (!$mensajeCliente) return;
    $mensajeCliente.textContent = texto || '';
    $mensajeCliente.classList.toggle('d-none', !texto);
  };

  const mapCliente = cliente => {
    const nombre = (cliente.nombreCompleto ??
      `${cliente.nomCliente ?? cliente.nombre ?? ''} ${cliente.apeCliente ?? cliente.apellido ?? ''}`.trim()).trim();
    const dni = (cliente.dni ?? cliente.documento ?? cliente.cuit ?? '').toString();
    const id = cliente.idCliente ?? cliente.id ?? cliente.codigo ?? null;
    return { id, nombre, dni };
  };

  function poblarSelectClientes(lista) {
    $selectCliente.options.length = 0;
    $selectCliente.add(new Option('— Seleccionar cliente—', ''));
    for (const cliente of lista) {
      if (cliente.id == null) continue;
      const opcion = new Option(`${cliente.nombre} — ${cliente.dni}`, cliente.id);
      opcion.dataset.nombre = cliente.nombre;
      opcion.dataset.dni = cliente.dni;
      $selectCliente.add(opcion);
    }
  }

  let datosCargados = false;
  let buscando = false;
  let clientesCache = [];

  function mostrarTodosClientes() {
    mostrarMensajeCliente('');
    if (datosCargados && clientesCache.length) {
      poblarSelectClientes(clientesCache);
    } else {
      cargarClientes();
    }
    $selectCliente.value = '';
  }

  async function cargarClientes() {
    if (buscando || datosCargados) return;

    mostrarMensajeCliente('');
    $selectCliente.options.length = 0;
    $selectCliente.add(new Option('Cargando clientes…', ''));

    try {
      const url = `${$apiUrl}/Clientes`;
      const response = await fetch(url, {
        headers: authHeaders({ Accept: 'application/json' }),
        cache: 'no-store'
      });

      if (!response.ok) throw new Error('HTTP ' + response.status);

      const data = await response.json();
      const lista = (Array.isArray(data) ? data : data?.items || data?.value || [])
        .map(mapCliente)
        .filter(item => item.id != null)
        .sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));

      clientesCache = lista;
      poblarSelectClientes(lista);
      datosCargados = true;
    } catch (error) {
      console.error(error);
      $selectCliente.options.length = 0;
      $selectCliente.add(new Option('Error al cargar', ''));
      mostrarMensajeCliente('No se pudieron cargar los clientes');
    }
  }

  async function buscarClientesPorNombreServidor(nombre) {
    const termino = (nombre || '').trim();
    if (!termino) {
      mostrarMensajeCliente('Ingrese un nombre');
      return;
    }

    buscando = true;
    mostrarMensajeCliente('');
    $selectCliente.options.length = 0;
    $selectCliente.add(new Option('Buscando…', ''));

    try {
      const url = `${$apiUrl}/Clientes/buscar/${encodeURIComponent(termino)}`;
      const response = await fetch(url, {
        headers: authHeaders({ Accept: 'application/json' }),
        cache: 'no-store'
      });

      if (response.status === 404) {
        poblarSelectClientes([]);
        mostrarMensajeCliente('Sin resultados');
        return;
      }
      if (!response.ok) throw new Error('HTTP ' + response.status);

      const data = await response.json();
      const lista = (Array.isArray(data) ? data : [data])
        .map(mapCliente)
        .filter(item => item.id != null);

      if (!lista.length) {
        poblarSelectClientes([]);
        mostrarMensajeCliente('Sin resultados');
        return;
      }

      poblarSelectClientes(lista);

      if (lista.length === 1) {
        $selectCliente.value = lista[0].id;
        if (window.setMsg) window.setMsg('');
        if (window.lockUI) {
          window.lockUI({
            idCliente: lista[0].id,
            nombreCompleto: lista[0].nombre,
            dni: lista[0].dni
          });
        }
      } else {
        mostrarMensajeCliente(`${lista.length} resultados. Elegí uno de la lista.`);
      }

      datosCargados = true;
    } catch (error) {
      console.error(error);
      poblarSelectClientes([]);
      mostrarMensajeCliente('Error al buscar');
    } finally {
      buscando = false;
    }
  }

  $selectCliente.addEventListener('focus', cargarClientes);
  $selectCliente.addEventListener('click', cargarClientes);

  if ($filtroCliente) {
    $filtroCliente.addEventListener('input', () => {
      const termino = ($filtroCliente.value || '').trim();
      if (termino.length >= 3) {
        buscarClientesPorNombreServidor(termino);
      } else if (termino === '') {
        mostrarTodosClientes();
      }
    });

    $filtroCliente.addEventListener('keydown', event => {
      if (event.key === 'Enter') {
        event.preventDefault();
        const termino = ($filtroCliente.value || '').trim();
        if (termino.length > 0) {
          buscarClientesPorNombreServidor(termino);
        } else {
          mostrarTodosClientes();
        }
      }
    });
  }

  $selectCliente.addEventListener('change', () => {
    const opcion = $selectCliente.selectedOptions[0];
    if (!opcion || !opcion.value) return;
    if (window.setMsg) window.setMsg('');
    if (window.lockUI) {
      window.lockUI({
        idCliente: opcion.value,
        nombreCompleto: opcion.dataset.nombre,
        dni: opcion.dataset.dni
      });
    }
  });

  window.__syncClienteSelect = function syncClienteSelect(cliente) {
    if (!cliente) return;
    const id = (cliente.idCliente ?? cliente.id ?? '').toString();
    const dni = (cliente.dni ?? cliente.documento ?? cliente.cuit ?? '').toString();
    const nombre = (cliente.nombreCompleto ??
      `${cliente.nomCliente ?? cliente.nombre ?? ''} ${cliente.apeCliente ?? cliente.apellido ?? ''}`.trim()).trim();

    const existente = Array.from($selectCliente.options).find(
      opcion => opcion.value === id
    );
    if (!existente) {
      const nuevaOpcion = new Option(`${nombre} — ${dni}`, id);
      nuevaOpcion.dataset.nombre = nombre;
      nuevaOpcion.dataset.dni = dni;
      if ($selectCliente.options.length > 0) {
        $selectCliente.add(nuevaOpcion, 1);
      } else {
        $selectCliente.add(nuevaOpcion);
      }
    }

    $selectCliente.value = id;
    if ($filtroCliente) $filtroCliente.value = '';
  };

  window.__clearClienteSelect = function clearClienteSelect() {
    $selectCliente.value = '';
    if ($filtroCliente) $filtroCliente.value = '';
    mostrarMensajeCliente('');
  };
})();

(function setupDatosFactura() {
  const $apiUrl = $API_BASE;
  const $selectTipo = document.getElementById('tipoFactura');
  const $spanFecha = document.getElementById('fechaActual');
  const $spanProximoNumero = document.getElementById('nroFacturaProximo');
  const $spanNombreEmpleado = document.getElementById('empleadoLogueado');
  const $spanIngreso = document.getElementById('fechaIngreso');
  const $spanDniEmpleado = document.getElementById('dniEmpleado');
  const idEmpleado = sessionStorage.getItem('idEmpleado');
  const ingresoEpoch = sessionStorage.getItem('ingresoEmpleado');

  if (!$selectTipo && !$spanFecha && !$spanProximoNumero && !$spanNombreEmpleado) {
    return;
  }

  if (idEmpleado && $spanNombreEmpleado) {
    obtenerDatosEmpleado(idEmpleado, ingresoEpoch);
  }

  function formatIngresoEpoch(epochSeconds) {
    if (!epochSeconds) return '';
    const timestamp = Number(epochSeconds);
    if (!Number.isFinite(timestamp)) return '';
    const date = new Date(timestamp * 1000);
    return date.toLocaleString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  function obtenerDatosEmpleado(id, ingreso) {
    fetch(`${$apiUrl}/Empleados/id/${id}`, {
      headers: authHeaders({ Accept: 'application/json' })
    })
      .then(response => response.json())
      .then(data => {
        const nombre = data.nomEmp;
        const apellido = data.apeEmp;
        const dni = data.dni;
        const ingresoFormateado = formatIngresoEpoch(ingreso);

        if ($spanNombreEmpleado) {
          $spanNombreEmpleado.textContent = `${nombre} ${apellido}`;
        }
        if ($spanIngreso) {
          $spanIngreso.textContent = ingresoFormateado
            ? `Ingreso: ${ingresoFormateado}`
            : '';
        }
        if ($spanDniEmpleado) {
          $spanDniEmpleado.textContent = `Dni: ${dni}`;
        }
      })
      .catch(error => console.error('Error obteniendo empleado:', error));
  }

  function setFechaHoy() {
    if (!$spanFecha) return;
    const hoy = new Date();
    const textoFecha = hoy.toLocaleDateString('es-AR', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
    $spanFecha.textContent = textoFecha;
    window.facturaFecha = hoy.toISOString().split('T')[0];
  }

  function initTipoFactura() {
    if (!$selectTipo) return;
    window.facturaTipo = $selectTipo.value;
    $selectTipo.addEventListener('change', () => {
      window.facturaTipo = $selectTipo.value;
    });
  }

  async function setProximoNumero() {
    if (!$spanProximoNumero) return;
    try {
      $spanProximoNumero.textContent = 'Cargando…';
      const response = await fetch(`${$apiUrl}/Facturas/ultima`, {
        headers: authHeaders({ Accept: 'application/json' }),
        cache: 'no-store'
      });
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      const data = await response.json();
      const ultimo = Number(data?.nroFactura ?? data?.NroFactura ?? 0);
      const proximo = Number.isFinite(ultimo) ? ultimo + 1 : 1;
      $spanProximoNumero.textContent = proximo.toString();
      window.facturaNumeroPropuesto = proximo;
    } catch (error) {
      console.error('Error obteniendo última factura:', error);
      $spanProximoNumero.textContent = '—';
      window.facturaNumeroPropuesto = undefined;
    }
  }

  document.addEventListener('DOMContentLoaded', () => {
    setFechaHoy();
    initTipoFactura();
    setProximoNumero();
  });
})();

document.addEventListener('DOMContentLoaded', () => {
  const $botonGenerar = document.getElementById('btnGenerarFactura');
  if (!$botonGenerar) return;

  $botonGenerar.addEventListener('click', async () => {
    try {
      if (!$ventaCarrito.size) {
        await Swal.fire({
          title: 'Error',
          theme: 'bootstrap-5-dark',
          background: '#212529',
          color: '#f8f9fa',
          text: 'Agregá al menos un producto a la factura.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        return;
      }

      const idEmpleado = Number(sessionStorage.getItem('idEmpleado'));
      if (!idEmpleado) {
        await Swal.fire({
          title: 'Error',
          theme: 'bootstrap-5-dark',
          background: '#212529',
          color: '#f8f9fa',
          text: 'No se encontró el empleado logueado. Volvé a iniciar sesión.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        return;
      }

      const $selectCliente = document.getElementById('clienteSelect');
      const idCliente = Number($selectCliente?.value || 0);
      if (!idCliente) {
        await Swal.fire({
          title: 'Error',
          theme: 'bootstrap-5-dark',
          background: '#212529',
          color: '#f8f9fa',
          text: 'Seleccioná un cliente antes de generar la factura.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        return;
      }

      const tipoFactura =
        window.facturaTipo ||
        document.getElementById('tipoFactura')?.value ||
        'B';

      const detalles = Array.from($ventaCarrito.values()).map(item => ({
        idProducto: item.idProducto,
        cantidad: item.cantidad,
        observ: item.observ || ''
      }));

      const payload = {
        tipoFactura,
        idEmpleado,
        idCliente,
        detalles
      };

      const response = await fetch(`${$API_BASE}/facturas`, {
        method: 'POST',
        headers: authHeaders({
          'Content-Type': 'application/json',
          Accept: 'application/json'
        }),
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const text = await response.text().catch(() => '');
        console.error('Error HTTP', response.status, text);
        alert(`Error al generar la factura (HTTP ${response.status}). Revisá la consola.`);
        return;
      }

      const data = await response.json().catch(() => ({}));
      const idFactura = data.idFactura;

      if (!idFactura) {
        alert('La factura se generó, pero la API no devolvió idFactura.');
        location.reload();
        return;
      }

      try {
        const pdfResponse = await fetch(`${$API_BASE}/facturas/${idFactura}/pdf`, {
          method: 'GET',
          headers: authHeaders({ Accept: 'application/pdf' })
        });

        if (pdfResponse.ok) {
          const blob = await pdfResponse.blob();
          const url = window.URL.createObjectURL(blob);
          const enlace = document.createElement('a');

          await Swal.fire({
            title: 'Factura generada',
            text: 'La factura y el PDF se generaron correctamente.',
            icon: 'success',
            background: '#212529',
            color: '#f8f9fa',
            confirmButtonColor: '#0d6efd'
          });

          enlace.href = url;
          enlace.download = `Factura_${idFactura}.pdf`;
          document.body.appendChild(enlace);
          enlace.click();
          enlace.remove();
          window.URL.revokeObjectURL(url);
        } else {
          console.error('Error al obtener PDF', pdfResponse.status);
          await Swal.fire({
            title: 'Error',
            theme: 'bootstrap-5-dark',
            background: '#212529',
            color: '#f8f9fa',
            text: 'La factura se creó, pero hubo un problema al generar el PDF.',
            icon: 'error',
            confirmButtonText: 'Ok'
          });
        }
      } catch (error) {
        console.error('Error descargando PDF:', error);
        await Swal.fire({
          title: 'Error',
          theme: 'bootstrap-5-dark',
          background: '#212529',
          color: '#f8f9fa',
          text: 'La factura se creó, pero falló la descarga del PDF.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }

      location.reload();
    } catch (error) {
      console.error('Error inesperado al crear la factura:', error);
      alert('Error inesperado al generar la factura. Revisá la consola.');
    }
  });
});
