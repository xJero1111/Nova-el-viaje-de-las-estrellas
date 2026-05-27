# NOVA: Un viaje por las estrellas
**NOVA** es un videojuego de exploración y plataformas en 2D (scroll lateral) desarrollado en Unity. Este proyecto adapta el universo narrativo de **NOVA** en una experiencia interactiva y lúdica, donde el jugador controla a una pequeña exploradora espacial que debe recolectar Brillos estelares y superar peligros para alcanzar su destino final. Este video juego hace parte del Proyecto Nova `[https://novaviajeporlasestrellaseafit.github.io/Nova/index.html]` Universo narrativo hecho para la fundación Funavid. Hecho en la clase Reto MediaLab 1: Convergencias. 

---

## Requisitos del Proyecto (Cumplimiento de Rúbrica)

###  Género y Diseño de Nivel
* **Género:** Platformer 2D con desplazamiento horizontal. Nova cuenta con un set dinámico de movimientos (Caminar, Saltar, Doble Saltar, Dash, Planeo y Escudo).
* **Tilemaps:** El entorno visual y las plataformas del mapa fueron construidos utilizando el sistema de **Tilemaps 2D** de Unity, estructurando de manera limpia los niveles de césped, minas y espacio.
* **Animaciones:** El personaje principal y los elementos del entorno poseen animaciones fluidas controladas por un `Animator` sincronizado con las mecánicas del juego.

###  Mecánicas de Juego
* **Coleccionables Múltiples:** El nivel integra objetos que otorgan puntuaciones:
 **Brillos Especiales:** Otorgan una puntuación al ser recolectados.
* **Condición de Victoria:** Se activa al alcanzar e interactuar con el **Conejo Espacial** en la tarjeta final del nivel, congelando el juego y abriendo la pantalla de cierre.
* **Condición de Derrota:** El jugador puede morir al perder sus puntos de salud por colisión con enemigos (`EnemyController`) o al caer en zonas de muerte instantánea (`InstantKillZone`) como pinchos, lava y vacíos.
* **Físicas y Colisiones:** Gestionadas por el motor de físicas 2D de Unity a través de un componente `Rigidbody2D` y `CapsuleCollider2D` en el jugador para interactuar con los triggers del mapa.

###  Interfaz de Usuario (UI)
* **Menú Principal (Escena "UI"):** Interfaz customizada con botones funcionales para:
  * **Jugar:** Carga la escena del juego (`"Juego"`).
  * **Créditos:** Muestra un panel superpuesto con los autores del juego y el botón de cerrar.
  * **Configuración:** Panel visual ("dummy") simulando opciones de audio y pantalla con botón de cerrar.
  * **Salir:** Cierra la aplicación de forma limpia.
* **HUD e Interfaz en Juego:**
  * **Panel de Instrucciones Inicial:** Al iniciar el nivel, el juego se pausa automáticamente mostrando los controles. Cuenta con un botón para cerrar y reanudar la partida.
  * **Contadores:** HUD visible en tiempo real que muestra la salud (corazones).
  * **Paneles de Fin de Partida:**
    * *Panel de Muerte:* Extrictamente restringido por la fundación Funavid al ser un producto dirigido para publico en tratamiento Oncologico. Cuando el jugador muere reaparece instantaneamente en el ultimo Check point.
    * *Panel de Victoria:* Aparece al tocar el final, deshabilitando el cierre por inputs y ofreciendo un botón directo para regresar al menú principal.
* **Diseño Visual (Skin):** Todos los paneles, tipografías (TextMeshPro) y botones utilizan sprites e imágenes personalizadas acordes a la estética del juego. Los botones cuentan con estados visuales diferenciados para **Normal**, **Resaltado (Hover)** y **Presionado**.

---

## Características Adicionales Investigadas e Implementadas

### 1. Mensajes y Tutoriales Contextuales Reutilizables (`ContextTutorialTrigger`)
Se investigó un sistema modular de triggers independientes que pausa los inputs del `PlayerController` y congela el `Rigidbody2D` en seco al entrar en un área. Inyecta dinámicamente texto a un componente `TextMeshPro` y activa un panel único. Mediante el sistema de inputs, detecta **cualquier tecla** para cerrarse y devolver de inmediato el control al jugador.

### 2. Módulo de Habilidades y Acompañante Flotante (`AbilityModule` & `CompanionFollow`)
Se implementó una arquitectura para desacoplar el control del jugador de un sistema de acompañamiento visual. El script del compañero calcula una interpolación suavizada para seguir a Nova a una distancia fija, mientras que un efecto matemático senoidal simula una flotación orgánica en el eje Y que reacciona visualmente según las habilidades activas de Nova (planeo, dash o escudo).

### 3. Persistencia de Datos mediante PlayerPrefs
Se investigó el almacenamiento ligero de datos locales a través de la API `PlayerPrefs` para gestionar el progreso en tiempo real de los checkpoints alcanzados y el registro de tutoriales ya vistos. Este sistema se conecta con el menú principal para limpiar por completo los datos guardados (`PlayerPrefs.DeleteAll()`) si el jugador decide iniciar una "Nueva Partida".

---

## Enlaces de Entrega

* **Link al Repositorio de GitHub:** `https://github.com/xJero1111/Nova-el-viaje-de-las-estrellas`
* **Ejecutable para Windows (.exe):** Disponible en la carpeta del proyecto o en la sección de *Releases*.
* **Link de Video en YouTube (Demostración de 5 min):** `[]`

---

## 👥 Créditos del Videojuego

* **Desarrollo, Programación y Diseño de Niveles:** Jerónimo Ríos Quintero.
* **Diseño de Personajes:** Samuel Gutiérrez Suárez *(Diseño visual de Nova, la nave espacial, los Brillos estelares y el arte de la tarjeta final)*.
* **Marco Académico:** Entregable para la materia de Lenguajes de Programación y MediaLab 1: Convergencias — Programas de Diseño Interactivo, Universidad EAFIT.