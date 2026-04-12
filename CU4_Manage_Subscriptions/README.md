# CU4 - Manage Subscriptions

Caso de uso para cambiar la suscripcion de un perfil tras validar el pago.

## Flujo

1. Recibe la solicitud de cambio de plan y los datos de pago.
2. Envía la peticion a la pasarela de pago.
3. Si el pago es aceptado, actualiza la suscripcion en `Perfil_Usuario`.
4. Asigna los privilegios del plan y genera un recibo electronico.
5. Devuelve exito a la UI.
6. Si el pago es rechazado, devuelve error y no modifica la base de datos.

## Planes

- Basic: menus semanales, ajuste al presupuesto, filtros basicos y necesidades medicas.
- Plus: Basic + intercambio de recetas y visualizacion detallada de macronutrientes.
- Familiar: Plus + multiples perfiles y analitica financiera con porcentaje y total ahorrado.