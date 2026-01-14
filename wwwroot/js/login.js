const $loginForm = document.getElementById('frmLogin');
const $userInput = document.getElementById('user');
const $passwordInput = document.getElementById('password');

$loginForm.addEventListener('submit', async event => {
  event.preventDefault();
  await loginUser($userInput.value, $passwordInput.value);
});

async function loginUser(username, password) {
  try {
    const response = await fetch('/api/Auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: username,
        password: password
      })
    });

    if (!response.ok) {
      console.error('Login HTTP', response.status);
      alert('Usuario o contraseña inválidos');
      return;
    }

    const data = await response.json();
    const tokenPayload = parseJwt(data.token);

    if (!tokenPayload) {
      alert('Token inválido');
      return;
    }

    let roles =
      tokenPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      tokenPayload.role ||
      [];

    if (!Array.isArray(roles)) {
      roles = [roles];
    }

    roles = roles.map(role => String(role).toLowerCase());

    sessionStorage.clear();
    sessionStorage.setItem('token', data.token);
    sessionStorage.setItem('idEmpleado', tokenPayload.sub);
    sessionStorage.setItem('ingresoEmpleado', tokenPayload.iat || '');
    sessionStorage.setItem('roles', JSON.stringify(roles));

    if (roles.includes('admin')) {
      window.location.href = './main.html';
    } else if (roles.includes('vendedor')) {
      window.location.href = './ventas.html';
    } else {
      logout();
    }
  } catch (error) {
    console.error('Error login:', error);
    alert('Error al conectar con el servidor.');
  }
}
