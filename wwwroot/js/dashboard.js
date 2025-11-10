const $API_BASE = '/api';

(function setupFacturasAccordion() {
  const $facturasApiUrl = `${$API_BASE}/Facturas/all`;
  const $accordionElement = document.getElementById('accordionFacturas');
  if (!$accordionElement) return;

  const formatMoney = value =>
    Number(value || 0).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });

  const formatDate = value => {
    if (!value) return '';
    const date = new Date(value);
    if (isNaN(date)) return String(value);
    return date.toLocaleDateString('es-AR', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
  };

  const escapeHtml = text =>
    String(text ?? '').replace(/[&<>"']/g, match => ({
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#39;'
    }[match]));

  function mapFactura(obj) {
    const cliente = obj.cliente ?? obj.Cliente ?? {};
    const empleado = obj.empleado ?? obj.Empleado ?? {};
    const detalles = obj.detalles ?? obj.Detalles ?? [];

    return {
      numero: obj.nroFactura ?? obj.NroFactura ?? obj.numero ?? obj.Numero ?? '(s/n)',
      tipo: obj.tipoFactura ?? obj.TipoFactura ?? '',
      fecha: obj.fechaFactura ?? obj.FechaFactura ?? obj.fecha ?? obj.Fecha,
      clienteNombre: cliente.nombreCompleto ?? cliente.NombreCompleto ?? 'Sin cliente',
      empleadoNombre: empleado.nombreCompleto ?? empleado.NombreCompleto ?? '',
      detalles: detalles.map(detalle => ({
        cantidad: detalle.cantidad ?? detalle.Cantidad ?? 0,
        precioUnitario: detalle.precioUnitario ?? detalle.PrecioUnitario ?? 0,
        productoNombre:
          detalle.producto?.nombreComercial ??
          detalle.Producto?.NombreComercial ??
          detalle.producto?.nombre ??
          detalle.Producto?.Nombre ??
          '—'
      }))
    };
  }

  const calcularTotal = detalles =>
    (detalles || []).reduce(
      (total, detalle) =>
        total + Number(detalle.precioUnitario || 0) * Number(detalle.cantidad || 0),
      0
    );

  function renderFacturas(facturas) {
    if (!Array.isArray(facturas) || facturas.length === 0) {
      $accordionElement.innerHTML =
        '<div class="text-center text-muted p-3">No hay facturas para mostrar.</div>';
      return;
    }

    const html = facturas
      .map((factura, index) => {
        const elementId = `f${index}-${escapeHtml(String(factura.numero))}`;
        const total = calcularTotal(factura.detalles);
        return `
          <div class="accordion-item mb-2">
            <h2 class="accordion-header" id="header-${elementId}">
              <button class="accordion-button bg-dark collapsed" type="button"
                      data-bs-toggle="collapse" data-bs-target="#collapse-${elementId}"
                      aria-expanded="false" aria-controls="collapse-${elementId}">
                <div class="d-flex w-100 justify-content-between align-items-center">
                  <div class="text-truncate">
                    <span class="me-2">Factura #${escapeHtml(String(factura.numero))}</span>
                    <span class="text-white">— ${escapeHtml(factura.clienteNombre)}</span>
                  </div>
                  <small class="text-white me-1 flex-shrink-0 ms-3">
                    ${escapeHtml(formatDate(factura.fecha))} • ${formatMoney(total)}
                  </small>
                </div>
              </button>
            </h2>
            <div id="collapse-${elementId}" class="accordion-collapse collapse"
                 aria-labelledby="header-${elementId}"
                 data-bs-parent="#accordionFacturas">
              <div class="accordion-body bg-dark pt-3">
                <ul class="mb-2">
                  <li><strong>Cliente:</strong> ${escapeHtml(factura.clienteNombre)}</li>
                  ${
                    factura.empleadoNombre
                      ? `<li><strong>Vendedor:</strong> ${escapeHtml(factura.empleadoNombre)}</li>`
                      : ''
                  }
                  <li><strong>Tipo:</strong> ${escapeHtml(factura.tipo)}</li>
                  <li><strong>Fecha:</strong> ${escapeHtml(formatDate(factura.fecha))}</li>
                  <li><strong>Total:</strong> ${formatMoney(total)}</li>
                </ul>
                ${
                  factura.detalles && factura.detalles.length
                    ? `
                  <div class="table-responsive">
                    <table class="table table-sm align-middle mb-0 table-dark-styled">
                      <thead class="table-secondary">
                        <tr>
                          <th>Producto</th>
                          <th class="text-center">Cant.</th>
                          <th class="text-center">P. Unit.</th>
                          <th class="text-end">Subtotal</th>
                        </tr>
                      </thead>
                      <tbody>
                        ${factura.detalles
                          .map(detalle => {
                            const subtotal =
                              Number(detalle.precioUnitario) *
                              Number(detalle.cantidad);
                            return `
                              <tr>
                                <td>${escapeHtml(detalle.productoNombre)}</td>
                                <td class="text-center">${Number(detalle.cantidad)}</td>
                                <td class="text-center">${formatMoney(detalle.precioUnitario)}</td>
                                <td class="text-end">${formatMoney(subtotal)}</td>
                              </tr>
                            `;
                          })
                          .join('')}
                      </tbody>
                    </table>
                  </div>`
                    : '<div class="text-muted">Sin detalles</div>'
                }
              </div>
            </div>
          </div>
        `;
      })
      .join('');

    $accordionElement.innerHTML = html;
  }

  async function cargarFacturas() {
    $accordionElement.innerHTML =
      '<div class="p-3"><span class="spinner-border spinner-border-sm me-2"></span>Cargando…</div>';

    try {
      const response = await fetch($facturasApiUrl, {
        headers: authHeaders({ Accept: 'application/json' })
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status} ${response.statusText}`);
      }

      const data = await response.json();
      const items = Array.isArray(data) ? data : data?.items || data?.value || [];
      renderFacturas(items.map(mapFactura));
    } catch (error) {
      console.error(error);
      $accordionElement.innerHTML =
        '<div class="alert alert-danger m-2">Error al cargar facturas.</div>';
    }
  }

  window.refrescarFacturas = cargarFacturas;
  cargarFacturas();
})();
