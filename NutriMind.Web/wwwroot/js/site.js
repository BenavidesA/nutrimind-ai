// Vanilla JS, sin dependencias (sin jQuery/Bootstrap) — dos comportamientos:
// 1) deshabilita el botón de envío hasta que el formulario sea válido (regla de UX de
//    "prevención de errores"), usando la Constraint Validation API nativa del navegador.
// 2) mostrar/ocultar contraseña en inputs marcados con [data-toggle-password].

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form[data-validate]').forEach(function (form) {
        var submitBtn = form.querySelector('button[type="submit"]');
        if (!submitBtn) return;

        function updateButtonState() {
            submitBtn.disabled = !form.checkValidity();
        }

        updateButtonState();
        form.addEventListener('input', updateButtonState);
        form.addEventListener('change', updateButtonState);
    });

    document.querySelectorAll('[data-toggle-password]').forEach(function (toggle) {
        toggle.addEventListener('click', function () {
            var targetId = toggle.getAttribute('data-toggle-password');
            var input = document.getElementById(targetId);
            if (!input) return;

            input.type = input.type === 'password' ? 'text' : 'password';
        });
    });
});
