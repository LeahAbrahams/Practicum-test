# מערכת ניהול קריאות שירות - API Documentation

## הרצת המערכת

### 1. יצירת בסיס הנתונים
```bash
# ב-Package Manager Console של Visual Studio:
Update-Database -Project DAL -StartupProject ServiceCallManagementSystem
```

### 2. הרצת האפליקציה
לחץ F5 ב-Visual Studio או:
```bash
dotnet run --project ServiceCallManagementSystem
```

---

## API Endpoints

### Offices (משרדים)

#### קבלת כל המשרדים
```http
GET /api/offices
```

**תגובה (200 OK):**
```json
[
  {
    "officeID": 1,
    "officeName": "משרד מיסוי ירושלים",
    "phone": "02-6543210",
    "officeType": "Field"
  }
]
```

#### קבלת משרד לפי ID
```http
GET /api/offices/1
```

#### יצירת משרד חדש
```http
POST /api/offices
Content-Type: application/json

{
  "officeName": "משרד מיסוי חיפה",
  "phone": "04-1234567",
  "officeType": "Field"
}
```

**תגובה בשגיאת Validation (400 Bad Request):**
```json
{
  "statusCode": 400,
  "message": "מספר טלפון חייב להכיל רק ספרות ומקפים",
  "details": "מספר טלפון חייב להכיל רק ספרות ומקפים"
}
```

#### 🆕 API 1: משרד ספציפי + כל קריאות השירות שלו
```http
GET /api/offices/1/with-service-calls
```

**תגובה (200 OK):**
```json
{
  "officeID": 1,
  "officeName": "משרד מיסוי ירושלים",
  "phone": "02-6543210",
  "officeType": "Field",
  "serviceCalls": [
    {
      "callID": 1,
      "officeID": 1,
      "officeName": "משרד מיסוי ירושלים",
      "description": "מקלדת לא עובדת",
      "status": "פתוח",
      "createdAt": "2024-01-15T00:00:00",
      "urgencyLevel": "דחוף"
    },
    {
      "callID": 2,
      "officeID": 1,
      "officeName": "משרד מיסוי ירושלים",
      "description": "מדפסת תקועה",
      "status": "בטיפול",
      "createdAt": "2024-01-20T00:00:00",
      "urgencyLevel": "לא רלוונטי"
    }
  ]
}
```

#### 🆕 API 2: כל המשרדים + כמות קריאות פתוחות
```http
GET /api/offices/with-open-calls-count
```

**תגובה (200 OK):**
```json
[
  {
    "officeID": 1,
    "officeName": "משרד מיסוי ירושלים",
    "phone": "02-6543210",
    "officeType": "Field",
    "openCallsCount": 3
  },
  {
    "officeID": 2,
    "officeName": "משרד מיסוי תל אביב",
    "phone": "03-7654321",
    "officeType": "Field",
    "openCallsCount": 1
  },
  {
    "officeID": 3,
    "officeName": "משרד ניהול מרכז",
    "phone": "03-9876543",
    "officeType": "Management",
    "openCallsCount": 0
  }
]
```

---

### Service Calls (קריאות שירות)

#### קבלת כל הקריאות
```http
GET /api/servicecalls
```

**תגובה (200 OK):**
```json
[
  {
    "callID": 1,
    "officeID": 1,
    "officeName": "משרד מיסוי ירושלים",
    "description": "מקלדת לא עובדת",
    "status": "פתוח",
    "createdAt": "2024-01-15T00:00:00",
    "urgencyLevel": "דחוף"
  }
]
```

**שים לב:** `urgencyLevel` הוא **Calculated Field** שמחושב אוטומטית:
- "דחוף" - פתוח מעל 7 ימים
- "בינוני" - פתוח מעל 3 ימים
- "רגיל" - פתוח פחות מ-3 ימים
- "לא רלוונטי" - סטטוס אחר

#### קבלת קריאות לפי משרד
```http
GET /api/servicecalls/office/1
```

#### יצירת קריאת שירות חדשה
```http
POST /api/servicecalls
Content-Type: application/json

{
  "officeID": 1,
  "description": "צריך עכבר חדש"
}
```

**תגובה בשגיאת Validation (400 Bad Request):**
```json
{
  "statusCode": 400,
  "message": "משרד עם מזהה 999 לא קיים במערכת",
  "details": "משרד עם מזהה 999 לא קיים במערכת"
}
```

#### עדכון סטטוס קריאה
```http
PATCH /api/servicecalls/1/status
Content-Type: application/json

{
  "newStatus": "בטיפול"
}
```

**ערכים מותרים:** "פתוח", "בטיפול", "טופל"

---

## Validation Rules (חוקי עסק)

### משרדים:
- ✅ מספר טלפון חייב להכיל רק ספרות ומקפים
- ✅ מספר טלפון הוא שדה חובה

### קריאות שירות:
- ✅ המשרד חייב להיות קיים במערכת
- ✅ תיאור הקריאה הוא שדה חובה
- ✅ סטטוס חייב להיות אחד מהערכים המותרים

---

## Global Exception Handling

כל השגיאות מטופלות ב-Middleware ומחזירות JSON מסודר:

**ValidationException → 400 Bad Request**
```json
{
  "statusCode": 400,
  "message": "הודעת שגיאה",
  "details": "הודעת שגיאה"
}
```

**Exception אחר → 500 Internal Server Error**
```json
{
  "statusCode": 500,
  "message": "הודעת שגיאה מקורית",
  "details": "שגיאה פנימית בשרת"
}
```

---

## JSON Format

כל התגובות ב-**camelCase** (הוגדר ב-Program.cs):
```json
{
  "officeID": 1,
  "officeName": "משרד מיסוי",
  "urgencyLevel": "דחוף"
}
```
