# Challenge Backend Alkemy C#
 
 Challenge de Alkemy para Backend C# Disney API.

## Lista de Endpoints:

> URL Base: **...**/api/**_referencia_**

### Auth
| Método | Referencia     | Propósito                                      |
|--------|----------------|------------------------------------------------|
| [POST] | /auth/register | **Registra un nuevo usuario en la base de datos.** |
| [POST] | /auth/login    | **Devuelve un JWT Token al validar las credenciales, para utilizar todas las funciones de la API.** |

### Character
| Método | Referencia     | Propósito                                      |
|--------|----------------|------------------------------------------------|
| [GET] | /characters | **Devuelve todos los personajes registrados en la base de datos (compatible con los siguientes parametros query: name, age y audiovisualworkId).** |
| [GET] | /characters/**id** | **Devuelve el personaje que corresponda con el Id recibido.** |
| [POST] | /characters | **Agrega un nuevo personaje a la base de datos.** |
| [PUT] | /characters/**id** | **Edita un personaje existente en la base de datos.** |
| [PUT] | /characters/**id**/audiovisualworks/**id** | **Asocia una obra audiovisual a un personaje ya existente.** |
| [DELETE] | /characters/**id** | **Elimina un personaje ya existente de la base de datos.** |

### AudiovisualWork
| Método | Referencia     | Propósito                                      |
|--------|----------------|------------------------------------------------|
| [GET] | /movies | **Devuelve todas las obras audiovisuales registradas en la base de datos  (compatible con los siguientes parametros query: name, genreId y order [ASC/DESC]).**|
| [GET] | /movies/**id** | **Devuelve la obra audiovisual que corresponda con el Id recibido.** |
| [POST] | /movies | **Agrega una nueva obra audiovisual a la base de datos.** |
| [PUT] | /movies/**id** | **Edita una obra audiovisual existente en la base de datos.** |
| [PUT] | /movies/**id**/genres/**id** | **Asocia un género a una obra audiovisual ya existente.** |
| [DELETE] | /movies/**id** | **Elimina una obra audiovisual ya existente de la base de datos.** |

### Genre
| Método | Referencia     | Propósito                                      |
|--------|----------------|------------------------------------------------|
| [GET] | /genres | **Devuelve todos los géneros registrados en la base de datos.** |
| [POST] | /genres | **Agrega un nuevo género a la base de datos.** |
| [PUT] | /genres/**id** | **Edita un género existente en la base de datos.** |
| [DELETE] | /genres/**id** | **Elimina un género ya existente de la base de datos.** |
