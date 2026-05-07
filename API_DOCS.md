# API Endpoint Documentation

Base URL: `http://localhost:5174`

All addresses in URL path segments are case-insensitive — they are normalized (lowercased, trimmed) before lookup.

---

## 1. GET /property/{address}

Returns violation history and scofflaw status for a given address.

**Request URL:** `GET /property/{address}`

**Request Payload:** None

**Path Parameter:**

| Parameter | Type   | Description                          |
|-----------|--------|--------------------------------------|
| `address` | string | Property address (e.g. `500 e 88th st`) |

---

**Response Code:** `200 OK`

**Response Payload:**
```json
{
    "lastViolationDate": "date (yyyy-MM-dd)",
    "totalViolations": "number",
    "violations": [
        {
            "date": "date (yyyy-MM-dd)",
            "code": "string | null",
            "status": "string | null",
            "description": "string | null",
            "inspectorComments": "string | null"
        }
    ],
    "isScofflaw": "boolean"
}
```

| Field               | Type            | Description                                                    |
|---------------------|-----------------|----------------------------------------------------------------|
| `lastViolationDate` | date            | Date of the most recent violation for this address             |
| `totalViolations`   | number          | Total number of violation records for this address             |
| `violations`        | array of object | All known violations; unordered                                |
| `violations[].date`              | date            | Violation date                               |
| `violations[].code`              | string \| null  | Violation code (e.g. `CN104035`)             |
| `violations[].status`            | string \| null  | Violation status (e.g. `OPEN`, `CLOSED`)     |
| `violations[].description`       | string \| null  | Short description of the violation           |
| `violations[].inspectorComments` | string \| null  | Inspector's notes; may be null               |
| `isScofflaw`        | boolean         | Whether this address appears on the scofflaw list              |

---

**Response Code:** `404 Not Found`

No violations found for the given address.

**Response Code:** `500 Internal Server Error`

Unexpected database error.

---

## 2. POST /property/{address}/comments/

Stores a user-submitted comment for a given address. The address must already exist in the violations table.

**Request URL:** `POST /property/{address}/comments/`

**Path Parameter:**

| Parameter | Type   | Description                          |
|-----------|--------|--------------------------------------|
| `address` | string | Property address (e.g. `4822 w melrose st`) |

**Request Payload:**
```json
{
    "author": "string",
    "comment": "string"
}
```

| Field     | Type   | Description                             |
|-----------|--------|-----------------------------------------|
| `author`  | string | Name or identifier of the commenter (required, non-empty) |
| `comment` | string | Comment text (required, non-empty)      |

---

**Response Code:** `201 Created`

**Response Payload:**
```json
{
    "message": "string"
}
```

| Field     | Type   | Description              |
|-----------|--------|--------------------------|
| `message` | string | Success confirmation text |

---

**Response Code:** `400 Bad Request`

Missing or empty `author` or `comment` field.

**Response Code:** `404 Not Found`

No violations exist for the given address; comments can only be attached to known properties.

**Response Code:** `500 Internal Server Error`

Unexpected database error.

---

## 3. GET /property/scofflaws/violations

Returns a list of scofflaw addresses that had at least one violation on or after the given date.

**Request URL:** `GET /property/scofflaws/violations?since={yyyy-MM-dd}`

**Request Payload:** None

**Query Parameter:**

| Parameter | Type   | Format       | Description                                          |
|-----------|--------|--------------|------------------------------------------------------|
| `since`   | string | `yyyy-MM-dd` | Return scofflaw addresses with violations on or after this date (required) |

---

**Response Code:** `200 OK`

**Response Payload:**
```json
{
    "addresses": [
        "string",
        "string"
    ]
}
```

| Field       | Type            | Description                                                            |
|-------------|-----------------|------------------------------------------------------------------------|
| `addresses` | array of string | Scofflaw addresses with at least one violation on or after `since`; may be empty |

---

**Response Code:** `400 Bad Request`

`since` parameter is missing or not in `yyyy-MM-dd` format.

**Response Code:** `500 Internal Server Error`

Unexpected database error.
