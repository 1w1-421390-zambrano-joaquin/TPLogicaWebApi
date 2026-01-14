const TOKEN_KEY = 'token';

function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64).split('').map(char =>
                '%' + ('00' + char.charCodeAt(0).toString(16)).slice(-2)
            ).join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

function getAuth() {
    const token = sessionStorage.getItem(TOKEN_KEY);
    if (!token) return null;

    const payload = parseJwt(token);
    if (!payload) return null;

    let roles =
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload.role ||
        [];

    if (!Array.isArray(roles)) roles = [roles];
    roles = roles.map(role => String(role).toLowerCase());

    return { token, payload, roles };
}

function requireAuth(rolesPermitidos = []) {
    const authInfo = getAuth();
    if (!authInfo) {
        window.location.href = '../index.html';
        return null;
    }

    if (rolesPermitidos.length) {
        const hasRole = authInfo.roles.some(role => rolesPermitidos.includes(role));
        if (!hasRole) {
            if (authInfo.roles.includes('vendedor')) {
                window.location.href = './ventas.html';
            } else {
                window.location.href = '../index.html';
            }
            return null;
        }
    }

    return authInfo;
}

function authHeaders(extraHeaders = {}) {
    const authInfo = getAuth();
    if (!authInfo?.token) return extraHeaders;

    return {
        ...extraHeaders,
        Authorization: 'Bearer ' + authInfo.token
    };
}

function logout() {
    sessionStorage.clear();
    window.location.href = '../index.html';
}
