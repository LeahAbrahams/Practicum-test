# מערכת ניהול קריאות שירות - סיכום פרויקט

## ארכיטקטורה: N-Tier (3 שכבות)

```
┌─────────────────────────────────────────┐
│   API Layer (ServiceCallManagementSystem)│
│   - Controllers                          │
│   - Middleware                           │
│   - Program.cs (DI Configuration)        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│   Business Logic Layer (BL)              │
│   - Services (OfficeService, etc.)       │
│   - Interfaces (IOfficeService, etc.)    │
│   - DTOs (Data Transfer Objects)         │
│   - Validation & Business Rules          │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│   Data Access Layer (DAL)                │
│   - Entities (Office, ServiceCall, etc.) │
│   - DbContext                            │
│   - Migrations                           │
└─────────────────────────────────────────┘
```

---

## מבנה הטבלאות

### 1. Offices (טבלה ראשית)
- OfficeID (PK)
- OfficeName
- Phone
- OfficeType

### 2. FieldOffices (משרדי שטח)
- OfficeID (PK, FK)
- HasPublicReception
- ReceptionAddress

### 3. ManagementOffices (משרדי ניהול)
- OfficeID (PK, FK)
- ProfessionalManager

### 4. ServiceCalls (קריאות שירות)
- CallID (PK)
- OfficeID (FK)
- Description
- Status
- CreatedAt

---

## קשרים בין טבלאות

- **Office ↔ FieldOffice**: One-to-Zero-or-One
- **Office ↔ ManagementOffice**: One-to-Zero-or-One
- **Office ↔ ServiceCalls**: One-to-Many
- **Cascade Delete**: על כל הקשרים

---

## תכונות מיוחדות

### 1. Calculated Field - UrgencyLevel
```csharp
public string UrgencyLevel
{
    get
    {
        var daysSinceCreated = (DateTime.Now - CreatedAt).Days;
        if (Status == "פתוח" && daysSinceCreated > 7) return "דחוף";
        if (Status == "פתוח" && daysSinceCreated > 3) return "בינוני";
        if (Status == "פתוח") return "רגיל";
        return "לא רלוונטי";
    }
}
```

### 2. Validation Rules
- מספר טלפון: רק ספרות ומקפים (Regex)
- משרד חייב להיות קיים לפני יצירת קריאה
- תיאור קריאה: שדה חובה
- סטטוס: רק ערכים מותרים (פתוח/בטיפול/טופל)

### 3. Global Exception Handling
- ValidationException → 400 Bad Request
- Exception אחר → 500 Internal Server Error
- JSON מסודר עם camelCase

---

## API Endpoints

### Offices:
1. `GET /api/offices` - כל המשרדים
2. `GET /api/offices/{id}` - משרד ספציפי
3. `POST /api/offices` - יצירת משרד
4. `GET /api/offices/{id}/with-service-calls` - משרד + קריאות
5. `GET /api/offices/with-open-calls-count` - משרדים + ספירה

### Service Calls:
1. `GET /api/servicecalls` - כל הקריאות
2. `GET /api/servicecalls/office/{officeId}` - קריאות לפי משרד
3. `POST /api/servicecalls` - יצירת קריאה
4. `PATCH /api/servicecalls/{id}/status` - עדכון סטטוס

---

## עקרונות ארכיטקטוניים שיושמו

### 1. Separation of Concerns
- כל שכבה אחראית על תחום אחד
- BL לא מכיר Connection String
- API לא מכיר Entities

### 2. Dependency Inversion
- תלות ב-Interfaces, לא במימושים
- Dependency Injection ב-Program.cs

### 3. Single Responsibility
- כל Service אחראי על תחום אחד
- כל DTO משרת מטרה ספציפית

### 4. Manual Mapping
- שליטה מלאה על המיפוי
- ביצועים טובים יותר מ-AutoMapper

---

## טכנולוגיות

- **.NET 8.0**
- **Entity Framework Core 8.0**
- **SQL Server LocalDB**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI**

---

## מבנה קבצים

```
ServiceCallManagementSystem/
├── DAL/
│   ├── Entities/
│   │   ├── Office.cs
│   │   ├── FieldOffice.cs
│   │   ├── ManagementOffice.cs
│   │   └── ServiceCall.cs
│   ├── DB/
│   │   └── ServiceCallDbContext.cs
│   └── Migrations/
│
├── BL/
│   ├── DTOs/
│   │   ├── OfficeDTO.cs
│   │   ├── ServiceCallDTO.cs
│   │   ├── CreateServiceCallDTO.cs
│   │   ├── OfficeWithServiceCallsDTO.cs
│   │   └── OfficeWithOpenCallsCountDTO.cs
│   ├── Interfaces/
│   │   ├── IOfficeService.cs
│   │   └── IServiceCallService.cs
│   ├── Services/
│   │   ├── OfficeService.cs
│   │   └── ServiceCallService.cs
│   └── Exceptions/
│       └── ValidationException.cs
│
└── ServiceCallManagementSystem/
    ├── Controllers/
    │   ├── OfficesController.cs
    │   └── ServiceCallsController.cs
    ├── Middleware/
    │   └── GlobalExceptionMiddleware.cs
    └── Program.cs
```

---

## הרצה ובדיקה

1. **Build**: `dotnet build` ✅
2. **Migration**: `dotnet ef database update` ✅
3. **Run**: `dotnet run` ✅
4. **Test**: Swagger UI ✅

---

## נקודות חוזק

✅ ארכיטקטורה נקייה ומסודרת  
✅ הפרדת שכבות מלאה  
✅ Validation מקיף  
✅ Exception Handling גלובלי  
✅ Async/Await לביצועים  
✅ Calculated Fields  
✅ RESTful API  
✅ Swagger Documentation  

---

## אפשרויות הרחבה עתידיות

- 🔐 Authentication & Authorization
- 📊 Logging (Serilog)
- 🧪 Unit Tests
- 📦 Repository Pattern (אופציונלי)
- 🔄 AutoMapper (אופציונלי)
- 📈 Monitoring & Health Checks
