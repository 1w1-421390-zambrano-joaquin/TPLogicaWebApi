const API_BASE = 'https://localhost:7143/api';
let productoSeleccionadoId = null;

function showSuccessAlert(message) {
  return Swal.fire({
    title: 'Acción realizada con éxito',
    text: message,
    icon: 'success',
    background: '#212529',
    color: '#f8f9fa',
    confirmButtonColor: '#0d6efd'
  });
}
function showErrorAlert(message) {
  return Swal.fire({
    title: 'Error',
    text: message,
    icon: 'error',
    background: '#212529',
    color: '#f8f9fa',
    confirmButtonColor: '#0d6efd'
  });
}

async function buscarProductosPorNombre(productNameInput) {
  const productoSelect = document.getElementById('productoSelect');
  const productoSelectMsg = document.getElementById('productoSelectMsg');

  if (!productoSelect) return;

  const searchTerm = (productNameInput || '').trim();

  if (!searchTerm) {
    productoSelect.innerHTML = '<option value="">— Ingrese un nombre —</option>';
    if (productoSelectMsg) {
      productoSelectMsg.textContent = '';
      productoSelectMsg.classList.add('d-none');
    }
    return;
  }

  productoSelect.innerHTML = '<option value="">Buscando…</option>';
  if (productoSelectMsg) {
    productoSelectMsg.textContent = '';
    productoSelectMsg.classList.add('d-none');
  }

  try {
    const response = await fetch(
      `${API_BASE}/Productos/buscar?nombre=${encodeURIComponent(searchTerm)}`,
      { headers: authHeaders({ Accept: 'application/json' }) }
    );

    if (!response.ok) throw new Error('HTTP ' + response.status);

    const productos = await response.json();

    if (!Array.isArray(productos) || productos.length === 0) {
      productoSelect.innerHTML = '<option value="">Sin resultados</option>';
      if (productoSelectMsg) {
        productoSelectMsg.textContent = 'No se encontraron productos con ese nombre.';
        productoSelectMsg.classList.remove('d-none');
      }
      return;
    }

    productoSelect.innerHTML = '<option value="">— Seleccionar producto —</option>';

    productos.forEach(product => {
      const option = document.createElement('option');
      option.value = product.idProducto;
      option.textContent = `${product.nombreComercial}`;
      productoSelect.appendChild(option);
    });

    if (productoSelectMsg) {
      if (productos.length === 1) {
        productoSelectMsg.textContent = 'Se encontró 1 producto. Seleccionalo de la lista.';
      } else {
        productoSelectMsg.textContent = `Se encontraron ${productos.length} productos. Seleccioná uno de la lista.`;
      }
      productoSelectMsg.classList.remove('d-none');
    }
  } catch (error) {
    console.error('Error al buscar productos:', error);
    productoSelect.innerHTML = '<option value="">Error al buscar</option>';
    if (productoSelectMsg) {
      productoSelectMsg.textContent = 'Error al buscar productos. Revisá la consola.';
      productoSelectMsg.classList.remove('d-none');
    }
    showErrorAlert('No se pudo buscar productos.');
  }
}

async function cargarProductoEnFormulario(productId) {
  if (!productId) return;

  try {
    const response = await fetch(`${API_BASE}/Productos/${productId}`, {
      headers: authHeaders({ Accept: 'application/json' })
    });

    if (!response.ok) {
      if (response.status === 404) {
        await showErrorAlert('Producto no encontrado.');
        return;
      }
      throw new Error('HTTP ' + response.status);
    }

    const product = await response.json();

    const codigoProductoLabel = document.getElementById('codProd');
    if (codigoProductoLabel) {
      codigoProductoLabel.innerHTML =
        ' #Código producto: ' + `<span class="fw-bold">${productId}</span>`;
    }

    productoSeleccionadoId = product.idProducto;

    const codigoProductoInput = document.getElementById('codigoProducto');
    const loteInput = document.getElementById('lote');
    const nombreComercialInput = document.getElementById('nombreComercial');
    const proveedorInput = document.getElementById('proveedor');
    const stockInput = document.getElementById('stock');
    const precioUnitarioInput = document.getElementById('precioUnitario');
    const principioActivoInput = document.getElementById('principioActivo');
    const unidadMedidaInput = document.getElementById('unidadMedida');
    const contenidoCantidadInput = document.getElementById('contenidoCantidad');
    const fechaVencimientoInput = document.getElementById('fVencimiento');

    if (codigoProductoInput) codigoProductoInput.value = product.idProducto ?? '';
    if (loteInput) loteInput.value = product.nroLote ?? '';
    if (nombreComercialInput) nombreComercialInput.value = product.nombreComercial ?? '';
    if (proveedorInput) proveedorInput.value = product.proveedor ?? '';
    if (stockInput) stockInput.value = product.stock ?? 0;
    if (precioUnitarioInput) precioUnitarioInput.value = product.precioUnitario ?? 0;
    if (principioActivoInput) principioActivoInput.value = product.principioActivo ?? '';
    if (unidadMedidaInput) unidadMedidaInput.value = product.unidadMedida ?? '';
    if (contenidoCantidadInput) contenidoCantidadInput.value = product.contenidoCantidad ?? '';

    if (fechaVencimientoInput && product.fVencimiento) {
      fechaVencimientoInput.value = String(product.fVencimiento).slice(0, 10);
    }

    await showSuccessAlert(
      'Producto cargado. Ahora podés modificar los datos (el código es solo lectura).'
    );
    buscarProductosPorNombre('');
  } catch (error) {
    console.error('Error al cargar producto:', error);
    await showErrorAlert('Error al cargar los datos del producto.');
  }
}

async function enviarUpdateProducto(action) {
  if (!productoSeleccionadoId) {
    await showErrorAlert('Primero seleccioná un producto desde el buscador.');
    return;
  }

  const nombreComercialInput = document.getElementById('nombreComercial');
  const principioActivoInput = document.getElementById('principioActivo');
  const contenidoCantidadInput = document.getElementById('contenidoCantidad');
  const unidadMedidaInput = document.getElementById('unidadMedida');
  const loteInput = document.getElementById('lote');
  const fechaVencimientoInput = document.getElementById('fVencimiento');
  const stockInput = document.getElementById('stock');
  const proveedorInput = document.getElementById('proveedor');
  const precioUnitarioInput = document.getElementById('precioUnitario');

  const nombreComercial = nombreComercialInput?.value.trim();
  const principioActivo = principioActivoInput?.value.trim();
  const unidadMedida = unidadMedidaInput?.value.trim();
  const proveedor = proveedorInput?.value.trim();
  const contenidoCantidad =
    contenidoCantidadInput?.value ? Number(contenidoCantidadInput.value) : null;
  const numeroLote = loteInput?.value ? Number(loteInput.value) : null;
  const fechaVencimiento = fechaVencimientoInput?.value || null;
  const stockForm = stockInput?.value ? Number(stockInput.value) : 0;
  const precio = precioUnitarioInput?.value ? Number(precioUnitarioInput.value) : 0;

  if (!nombreComercial || !proveedor) {
    await showErrorAlert('Nombre comercial y proveedor son obligatorios.');
    return;
  }
  if (numeroLote == null || numeroLote <= 0) {
    await showErrorAlert('El número de lote debe ser mayor a 0.');
    return;
  }
  if (contenidoCantidad == null || contenidoCantidad <= 0) {
    await showErrorAlert('La cantidad de contenido debe ser mayor a 0.');
    return;
  }
  if (precio < 0) {
    await showErrorAlert('El precio no puede ser negativo.');
    return;
  }
  if (stockForm < 0) {
    await showErrorAlert('El stock no puede ser negativo.');
    return;
  }

  let stockFinal = stockForm;
  let nombreFinal = nombreComercial;

  if (action === 'bajaProducto') {
    stockFinal = 0;
    nombreFinal = `${nombreComercial} [Eliminado]`;
  }

  const productoDto = {
    nombreComercial: nombreFinal,
    principioActivo: principioActivo,
    contenidoCantidad: contenidoCantidad,
    unidadMedida: unidadMedida,
    nroLote: numeroLote,
    fVencimiento: fechaVencimiento,
    stock: stockFinal,
    proveedor: proveedor,
    precio: precio
  };

  const confirmationMessage =
    action === 'bajaProducto'
      ? 'Se dará de baja este producto. ¿Está seguro?'
      : '¿Querés guardar los cambios de este producto?';

  const confirmResult = await Swal.fire({
    title: 'Confirmar',
    text: confirmationMessage,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí',
    cancelButtonText: 'No',
    background: '#212529',
    color: '#f8f9fa',
    confirmButtonColor: '#0d6efd',
    cancelButtonColor: '#6c757d'
  });

  if (!confirmResult.isConfirmed) {
    return;
  }

  try {
    const response = await fetch(
      `${API_BASE}/Productos/${productoSeleccionadoId}`,
      {
        method: 'PUT',
        headers: authHeaders({
          'Content-Type': 'application/json',
          Accept: 'application/json'
        }),
        body: JSON.stringify(productoDto)
      }
    );

    if (!response.ok) {
      const errorText = await response.text().catch(() => '');
      console.error('Error PUT producto:', response.status, errorText);

      if (response.status === 400) {
        await showErrorAlert(
          'Datos inválidos. Revisá los campos del formulario.'
        );
      } else if (response.status === 404) {
        await showErrorAlert('El producto ya no existe.');
      } else {
        await showErrorAlert(
          'Error al actualizar el producto, revisá si las fechas y/o campos poseen valores válidos.'
        );
      }
      return;
    }

    const successText =
      action === 'bajaProducto'
        ? 'Producto dado de baja.'
        : 'Producto actualizado correctamente.';

    await showSuccessAlert(successText);
    location.reload();
  } catch (error) {
    console.error('Error inesperado en PUT:', error);
    await showErrorAlert('Error inesperado al actualizar el producto.');
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const productoFilterInput = document.getElementById('productoFilter');
  const productoSelect = document.getElementById('productoSelect');
  const updateProductForm = document.getElementById('form-modificar-producto');

  if (productoFilterInput) {
    productoFilterInput.addEventListener('input', () => {
      const filterValue = productoFilterInput.value;
      if (filterValue.length >= 1) {
        buscarProductosPorNombre(filterValue);
      } else {
        buscarProductosPorNombre('');
      }
    });

    productoFilterInput.addEventListener('keydown', event => {
      if (event.key === 'Enter') {
        event.preventDefault();
        buscarProductosPorNombre(productoFilterInput.value);
      }
    });
  }

  if (productoSelect) {
    productoSelect.addEventListener('change', () => {
      const selectedId = Number(productoSelect.value);
      if (selectedId) {
        cargarProductoEnFormulario(selectedId);
      }
    });
  }

  if (updateProductForm) {
    updateProductForm.addEventListener('submit', event => {
      event.preventDefault();
      const submitButton = event.submitter;
      const action = submitButton?.value;
      enviarUpdateProducto(action);
    });
  }
});
