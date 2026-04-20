document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('register-form');
  const result = document.getElementById('register-result');

  // Backend CU1 (Minimal API)
  const API_BASE = 'http://localhost:5000';

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    result.textContent = '';

    const body = {
      nombre: document.getElementById('nombre').value,
      apellidos: document.getElementById('apellidos').value,
      nombreUsuario: document.getElementById('nombreUsuario').value,
      gmail: document.getElementById('gmail').value,
      telefono: document.getElementById('telefono').value,
      contrasena: document.getElementById('contrasena').value,
    };

    try {
      const res = await fetch(`${API_BASE}/api/profiles`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });

      const data = await res.json().catch(() => null);

      if (!res.ok) {
        result.style.color = '#b91c1c';
        result.textContent = (data && data.message) ? data.message : 'No se pudo crear el perfil.';
        return;
      }

      result.style.color = '#15803d';
      result.textContent = `Perfil creado. Id: ${data.profileId}. Usuario: ${data.nombreUsuario}`;
      form.reset();
    } catch {
      result.style.color = '#b91c1c';
      result.textContent = 'Error de red al conectar con el servidor.';
    }
  });
});
