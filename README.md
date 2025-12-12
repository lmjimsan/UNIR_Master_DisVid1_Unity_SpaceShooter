# Space Shooter 2D · Proyecto educativo

> Shooter vertical en Unity centrado en progresión por oleadas, uso táctico de bombas y gestión de vidas. Documentado y listo para exhibir.

## Visión general
- Objetivo: sobrevivir y limpiar todas las oleadas de enemigos en el nivel actual.
- Lo que hace especial el proyecto: mecánicas adicionales (bombas globales, respawn con invulnerabilidad breve, HUD vivo), documentación extensa y arquitectura lista para crecer.
- Público: entrega académica 2025, código claro para aprendizaje y extensión.

## Gameplay en 30 segundos
- Movimiento con WASD; disparo continuo; bomba de emergencia que limpia la pantalla con stock limitado.
- Vidas visibles en HUD; al quedar sin vidas aparece derrota con opción de reintento inmediato.
- Oleadas crecientes: más naves, mayor velocidad y cadencia; al completar todas se muestra victoria.

## Controles
```
W / A / S / D   Mover nave (tanbién sirven las flechas)
Space           Disparo continuo (tanbién sirve el botón izquierdo del ratón)
B               Bomba (limpia/daña a todos en pantalla, stock limitado, también sirve el botón derecho del ratón)
Esc / P         Pausa (si está habilitada en escena)
Any key         Avanza desde pantalla de inicio y reintenta tras derrota
```

## Flujo de juego y pantallas
1) Pantalla de inicio: logo + instrucciones; pulsa cualquier tecla para empezar.
2) Gameplay: HUD con vidas, bombas, oleada y puntuación; fondo animado y efectos.
3) Victoria: aparece al superar todas las oleadas; muestra marcador y opción de volver al menú.
4) Derrota: se activa al agotar vidas; permite reintentar la partida o regresar al menú.

## Cómo se gana, se pierde y se reintenta
- Ganas: destruyendo a todos los enemigos de la última oleada del nivel.
- Pierdes: al colisionar o recibir proyectiles hasta agotar el contador de vidas.
- Reintento: desde pantalla de derrota o mediante cualquier tecla si el reinicio rápido está activo; restablece vidas, bombas y reinicia la secuencia de oleadas.

## Bombas, vidas y oleadas
- Bombas: recurso escaso; al usarlas limpian o dañan todo enemigo activo. Guardadas en el HUD; pueden recargarse con pickups/eventos si la escena los define.
- Vidas: contador visible; tras perder una, el jugador reaparece con invulnerabilidad breve para evitar colisiones instantáneas.
- Oleadas: definidas en un gestor que escala cantidad, velocidad y cadencia. El avance de oleada se produce al eliminar todos los enemigos presentes.

## Ficheros y lógica principal
- Assets/Scripts/Player*: control de input, disparo, bombas, vidas, respawn y colisiones del jugador.
- Assets/Scripts/Enemy*: comportamiento de enemigos y sus disparos (p. ej. EnemyShoot_Type1.cs) más spawn y patrones de movimiento.
- Assets/Scripts/Wave* o Managers: secuenciación de oleadas y parametrización de dificultad.
- Assets/Scripts/UI* o HUD*: renderizado de vidas, bombas, oleada, puntuación y pantallas de inicio/victoria/derrota.
- Managers auxiliares: música persistente, checklist de estados y persistencia de récord.

## Cómo jugar (rápido)
1) Abrir la escena principal (Assets/Scenes/MainScene.unity) y pulsar Play.
2) Mover con WASD y mantener Space para disparar.
3) Usar B solo en situaciones críticas; el stock es limitado.
4) Completar todas las oleadas para ganar; si pierdes, reintenta desde la pantalla de derrota.

## Próximas versiones
- Niveles: progresión por capítulos con ambientación y escalado propio.
- Power ups: escudos, velocidad, daño extra, cadencia y bombas adicionales.
- Nuevos enemigos y estrategias: patrones dirigidos, formaciones mixtas, enjambres y unidades de soporte.
- Jefes de final de mundo: fases múltiples, puntos débiles y mecánicas especiales.
- Metajuego: leaderboard online, ajustes de dificultad y guardado de progreso.

## Crédito y licencia
Proyecto educativo creado por Luis Miguel Jiménez (2025). 
