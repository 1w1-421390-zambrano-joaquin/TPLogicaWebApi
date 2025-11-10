const $API_BASE = '/api';

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

function showConfirmAlert(message) {
  return Swal.fire({
    title: 'Confirmar',
    text: message,
    icon: 'question',
    showCancelButton: true,
    confirmButtonText: 'Guardar',
    cancelButtonText: 'Cancelar',
    background: '#212529',
    color: '#f8f9fa',
    confirmButtonColor: '#0d6efd',
    cancelButtonColor: '#6c757d'
  });
}

function getProductoInsertDto() {
  const $nombreComercial = document.getElementById('nombreComercial');
  const $principioActivo = document.getElementById('principioActivo');
  const $contenidoCantidad = document.getElementById('contenidoCantidad');
  const $unidadMedida = document.getElementById('unidadMedida');
  const $numeroLote = document.getElementById('nroLote');
  const $fechaVencimiento = document.getElementById('fVencimiento');
  const $stock = document.getElementById('stock');
  const $proveedor = document.getElementById('proveedor');
  const $precioUnitario = document.getElementById('precioUnitario');

  return {
    nombreComercial: $nombreComercial?.value.trim() || '',
    principioActivo: $principioActivo?.value.trim() || '',
    contenidoCantidad: $contenidoCantidad?.value ? Number($contenidoCantidad.value) : NaN,
    unidadMedida: $unidadMedida?.value.trim() || '',
    nroLote: $numeroLote?.value ? Number($numeroLote.value) : NaN,
    fVencimiento: $fechaVencimiento?.value || '',
    stock: $stock?.value ? Number($stock.value) : NaN,
    proveedor: $proveedor?.value.trim() || '',
    precio: $precioUnitario?.value ? Number($precioUnitario.value) : NaN
  };
}

function validarProducto(productDto) {
    if (!productDto.nombreComercial) {
        return 'El nombre comercial es obligatorio.';
    }
    if (!productDto.principioActivo) {
        return 'El principio activo es obligatorio.';
    }
    if (!Number.isFinite(productDto.contenidoCantidad) || productDto.contenidoCantidad <= 0) {
        return 'El contenido debe ser mayor a 0.';
    }
    if (!productDto.unidadMedida) {
        return 'La unidad de medida es obligatoria.';
    }
    if (/^\d+([.,]\d+)?$/.test(productDto.unidadMedida)) {
        return 'La unidad de medida no puede ser solo números.';
    }
    if (!Number.isInteger(productDto.nroLote) || productDto.nroLote <= 0) {
        return 'El número de lote debe ser mayor a 0.';
    }
    if (!productDto.fVencimiento) {
        return 'La fecha de vencimiento es obligatoria.';
    }
    if (!Number.isInteger(productDto.stock) || productDto.stock <= 0) {
        return 'El stock debe ser un número mayor a 0.';
    }
    if (!productDto.proveedor) {
        return 'El proveedor es obligatorio.';
    }
    if (!Number.isFinite(productDto.precio) || productDto.precio <= 0) {
        return 'El precio debe ser mayor a 0.';
    }
    return null;
}


async function createProduct(event) {
  event.preventDefault();

  const productDto = getProductoInsertDto();
  const validationError = validarProducto(productDto);

  if (validationError) {
    await showErrorAlert(validationError);
    return;
  }

  const confirmResult = await showConfirmAlert('¿Deseás guardar este nuevo producto?');
  if (!confirmResult.isConfirmed) return;

  try {
    const response = await fetch(`${$API_BASE}/Productos`, {
      method: 'POST',
      headers: authHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }),
      body: JSON.stringify(productDto)
    });

    if (!response.ok) {
      const responseText = await response.text().catch(() => '');
      console.error('Error POST producto:', response.status, responseText);

      if (response.status === 400) {
        await showErrorAlert('Los datos no son válidos. Revisá los campos.');
      } else if (response.status === 409) {
        let responseBody = {};
        try {
          responseBody = await response.json();
        } catch {
          responseBody = {};
        }
        await showErrorAlert(
          responseBody.message || 'Ya existe un producto con ese nombre comercial.'
        );
      } else {
        await showErrorAlert(`Error al crear el producto (HTTP ${response.status}).`);
      }
      return;
    }

    await showSuccessAlert('Producto creado correctamente.');

    const $formInsertProduct = document.getElementById('form-insert-producto');
    if ($formInsertProduct) {
      $formInsertProduct.reset();
    }
  } catch (error) {
    console.error('Error inesperado en POST:', error);
    await showErrorAlert('Error inesperado al crear el producto.');
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const $formInsertProduct = document.getElementById('form-insert-producto');
  if ($formInsertProduct) {
    $formInsertProduct.addEventListener('submit', createProduct);
  }
});
