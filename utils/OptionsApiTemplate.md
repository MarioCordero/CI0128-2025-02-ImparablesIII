Claro, aquí tienes un README que explica cómo crear un componente básico en Vue.js usando Options API, junto con un prompt para que una IA pueda ayudarte a refactorizar cualquier componente hacia ese mismo formato uniforme.

---

# README: Cómo crear un componente en Vue.js con Options API

## Introducción

Vue.js es un framework progresivo para construir interfaces de usuario. En Vue, un **componente** es una instancia de Vue con opciones predefinidas como `data`, `methods`, `computed`, y `lifecycle hooks`.

Este README te explica cómo crear un componente básico usando la **Options API**, un estilo clásico y muy utilizado en Vue 2 y Vue 3, que organiza el código en secciones claras.

## Ejemplo básico de componente en Options API

```js
export default {
  data() {
    return {
      message: 'Hello, Vue!'
    };
  },
  computed: {
    reversedMessage() {
      return this.message.split('').reverse().join('');
    }
  },
  methods: {
    changeMessage(newMessage) {
      this.message = newMessage;
    }
  },
  mounted() {
    console.log('Component has been mounted!');
  }
};
```

### Explicación:

* `data`: Función que devuelve un objeto con el estado reactivo del componente.
* `computed`: Propiedades computadas, recalculan valores derivados de manera reactiva.
* `methods`: Funciones para manejar eventos o lógica de negocio.
* `mounted`: Hook del ciclo de vida que se ejecuta cuando el componente se monta en el DOM.

## Cómo usar este componente

1. Guarda el código en un archivo `.vue` o en un archivo JavaScript que exporte este objeto.
2. Importa el componente en tu proyecto Vue y úsalo dentro de tu plantilla.

```vue
<template>
  <div>
    <p>{{ message }}</p>
    <p>{{ reversedMessage }}</p>
    <button @click="changeMessage('Nuevo mensaje')">Cambiar mensaje</button>
  </div>
</template>

<script>
import MiComponente from './MiComponente.js';

export default {
  components: {
    MiComponente
  }
};
</script>
```

---

# Prompt para refactorizar componentes a Options API uniforme

```
Eres un asistente experto en Vue.js. Refactoriza el siguiente componente para que use exclusivamente la Options API de Vue.js y siga la siguiente estructura clara y uniforme:

1. La opción `data` debe ser una función que retorne un objeto con las propiedades reactivas.
2. Las propiedades computadas deben estar agrupadas dentro de `computed`.
3. Los métodos deben estar agrupados dentro de `methods`.
4. Los hooks del ciclo de vida deben estar presentes y ordenados al final (por ejemplo, mounted, created).
5. El código debe estar limpio, indentado y sin funciones flecha para definir las opciones.
6. El componente debe exportarse por defecto con `export default`.

Refactoriza el siguiente código y devuelve únicamente el componente refactorizado, sin explicaciones adicionales:

---  
[Código del componente a refactorizar]  
---
```