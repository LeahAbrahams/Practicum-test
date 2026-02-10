# ERD - מערכת ניהול קריאות שירות

## 1. תרשים ERD (Entity Relationship Diagram)

```
┌─────────────────────────────────────────────────────────────────┐
│                         OFFICES (טבלה ראשית)                    │
│─────────────────────────────────────────────────────────────────│
│ PK  OfficeID          int                                        │
│     OfficeName        nvarchar(50)                               │
│     Phone             nvarchar(MAX)                              │
│     OfficeType        nvarchar(20)  ["Field" / "Management"]    │
└─────────────────────────────────────────────────────────────────┘
           │  1                                | N
           │                                   |
           │                                   |
           ├─────────────┐                     |
           │  1          │ 1                   | 1
           ▼             ▼                     ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│  FIELD_OFFICES   │  │ MANAGEMENT_      │  │  SERVICE_CALLS   │
│  (משרדי שטח)    │  │ OFFICES          │  │  (קריאות שירות) │
│──────────────────│  │ (משרדי הנהלה)   │  │──────────────────│
│ PK,FK OfficeID   │  │──────────────────│  │ PK  CallID       │
│       HasPublic  │  │ PK,FK OfficeID   │  │ FK  OfficeID     │
│       Reception  │  │       Professional│ │     Description  │
│       Reception  │  │       Manager    │  │     Status       │
│       Address    │  │                  │  │     CreatedAt    │
└──────────────────┘  └──────────────────┘  └──────────────────┘
                                                      
                                                      
```

---

## 2. קרדינליות (Cardinality)

### קשר 1: Offices ↔ FieldOffices
- **סוג:** One-to-Zero-or-One (1:1)
- **הסבר:** לא כל משרד הוא משרד שטח
- **מימוש:** OfficeID ב-FieldOffices הוא גם PK וגם FK

### קשר 2: Offices ↔ ManagementOffices
- **סוג:** One-to-Zero-or-One (1:1)
- **הסבר:** לא כל משרד הוא משרד הנהלה
- **מימוש:** OfficeID ב-ManagementOffices הוא גם PK וגם FK

### קשר 3: Offices ↔ ServiceCalls
- **סוג:** One-to-Many (1:N)
- **הסבר:** משרד אחד יכול לפתוח מספר קריאות שירות
- **מימוש:** OfficeID ב-ServiceCalls הוא FK בלבד

---

## 3. הגדרת טבלאות מפורטת

### טבלה 1: OFFICES (משרדים)
```sql
CREATE TABLE Offices (
    OfficeID        INT             PRIMARY KEY IDENTITY(1,1),
    OfficeName      NVARCHAR(50)    NOT NULL,
    Phone           NVARCHAR(MAX)   NOT NULL,
    OfficeType      NVARCHAR(20)    NOT NULL
        CHECK (OfficeType IN ('Field', 'Management'))
);
```

| עמודה | סוג | NULL? | הגבלות | הערות |
|-------|-----|-------|--------|-------|
| **OfficeID** | `int` | ❌ | PK, Identity | מזהה ייחודי אוטומטי |
| OfficeName | `nvarchar(50)` | ❌ | - | שם המשרד |
| Phone | `nvarchar(MAX)` | ❌ | - | מספר טלפון |
| OfficeType | `nvarchar(20)` | ❌ | CHECK | "Field" או "Management" |

---

### טבלה 2: FIELD_OFFICES (משרדי שטח)
```sql
CREATE TABLE FieldOffices (
    OfficeID            INT             PRIMARY KEY,
    HasPublicReception  BIT             NOT NULL DEFAULT 0,
    ReceptionAddress    NVARCHAR(MAX)   NULL,
    
    CONSTRAINT FK_FieldOffices_Offices 
        FOREIGN KEY (OfficeID) 
        REFERENCES Offices(OfficeID)
        ON DELETE CASCADE
);
```

| עמודה | סוג | NULL? | הגבלות | הערות |
|-------|-----|-------|--------|-------|
| **OfficeID** | `int` | ❌ | PK + FK | מפתח ראשי וזר |
| HasPublicReception | `bit` | ❌ | Default: 0 | האם יש קבלת קהל |
| ReceptionAddress | `nvarchar(MAX)` | ✅ | - | כתובת קבלת קהל (אופציונלי) |

**הערה חשובה:** OfficeID כאן הוא **גם PK וגם FK** - זה מבטיח One-to-One!

---

### טבלה 3: MANAGEMENT_OFFICES (משרדי הנהלה)
```sql
CREATE TABLE ManagementOffices (
    OfficeID            INT             PRIMARY KEY,
    ProfessionalManager NVARCHAR(50)    NOT NULL,
    
    CONSTRAINT FK_ManagementOffices_Offices 
        FOREIGN KEY (OfficeID) 
        REFERENCES Offices(OfficeID)
        ON DELETE CASCADE
);
```

| עמודה | סוג | NULL? | הגבלות | הערות |
|-------|-----|-------|--------|-------|
| **OfficeID** | `int` | ❌ | PK + FK | מפתח ראשי וזר |
| ProfessionalManager | `nvarchar(50)` | ❌ | - | שם המנהל המקצועי |

---

### טבלה 4: SERVICE_CALLS (קריאות שירות)
```sql
CREATE TABLE ServiceCalls (
    CallID      INT             PRIMARY KEY IDENTITY(1,1),
    OfficeID    INT             NOT NULL,
    Description NVARCHAR(MAX)   NOT NULL,
    Status      NVARCHAR(20)    NOT NULL DEFAULT 'פתוח'
        CHECK (Status IN ('פתוח', 'בטיפול', 'טופל')),
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_ServiceCalls_Offices 
        FOREIGN KEY (OfficeID) 
        REFERENCES Offices(OfficeID)
        ON DELETE CASCADE
);
```

| עמודה | סוג | NULL? | הגבלות | הערות |
|-------|-----|-------|--------|-------|
| **CallID** | `int` | ❌ | PK, Identity | מזהה קריאה ייחודי |
| OfficeID | `int` | ❌ | FK | המשרד שפתח את הקריאה |
| Description | `nvarchar(MAX)` | ❌ | - | תיאור הצורך (מקלדת/תחזוקה וכו') |
| Status | `nvarchar(20)` | ❌ | CHECK, Default | סטטוס הקריאה |
| CreatedAt | `datetime2` | ❌ | Default | תאריך ושעת יצירה |

---

## 4. אינדקסים מומלצים

```sql
-- אינדקס על OfficeID ב-ServiceCalls (לשאילתות מהירות)
CREATE INDEX IX_ServiceCalls_OfficeID 
    ON ServiceCalls(OfficeID);

-- אינדקס על Status (לסינון לפי סטטוס)
CREATE INDEX IX_ServiceCalls_Status 
    ON ServiceCalls(Status);

-- אינדקס מורכב (לשאילתות מורכבות)
CREATE INDEX IX_ServiceCalls_OfficeID_Status 
    ON ServiceCalls(OfficeID, Status);
```

---

## 5. דוגמאות נתונים

### Offices
| OfficeID | OfficeName | Phone | OfficeType |
|----------|------------|-------|------------|
| 1 | משרד חקירות עכו | 04-1234567 | Field |
| 2 | משרד מיסוי ירושלים | 02-7654321 | Field |
| 3 | משרד ניהול מרכז | 03-9876543 | Management |

### FieldOffices
| OfficeID | HasPublicReception | ReceptionAddress |
|----------|-------------------|------------------|
| 1 | 1 (כן) | רח' הגליל 10, עכו |
| 2 | 1 (כן) | רח' יפו 97, ירושלים |

### ManagementOffices
| OfficeID | ProfessionalManager |
|----------|---------------------|
| 3 | דוד כהן |

### ServiceCalls
| CallID | OfficeID | Description | Status | CreatedAt |
|--------|----------|-------------|--------|-----------|
| 1 | 1 | מקלדת חדשה | פתוח | 2024-02-10 |
| 2 | 1 | תיקון מדפסת | בטיפול | 2024-02-09 |
| 3 | 2 | עכבר אלחוטי | טופל | 2024-02-08 |

---

## 6. החלטות תכנוניות - הסברים

### ✅ למה Table Per Type (TPT)?
**החלטה:** טבלה ראשית (Offices) + 2 טבלאות נפרדות

**יתרונות:**
- ✅ אין שדות NULL מיותרים
- ✅ כל סוג משרד עם השדות הספציפיים שלו
- ✅ קל להוסיף סוגי משרדים נוספים
- ✅ Referential Integrity מובטח

**חלופות שנדחו:**
- ❌ Table Per Hierarchy (TPH) - יצר שדות NULL מיותרים
- ❌ Table Per Concrete (TPC) - קשה לשאילתות על כל המשרדים

---

### ✅ למה OfficeID הוא גם PK וגם FK?
**זה מבטיח One-to-One relationship!**

אם היינו עושים:
```sql
-- ❌ לא נכון:
FieldOfficeID INT PRIMARY KEY
OfficeID INT FOREIGN KEY
```
אז משרד אחד יכול להיות מקושר למספר FieldOffices!

---

### ✅ למה Cascade Delete?
```sql
ON DELETE CASCADE
```

**הסבר:** אם מוחקים משרד, אוטומטית נמחקים:
- הרשומה ב-FieldOffices/ManagementOffices
- כל הקריאות שלו ב-ServiceCalls

**זה הגיוני עסקית!**

---

### ✅ למה NVARCHAR ולא VARCHAR?
**NVARCHAR = Unicode = תומך עברית! 🇮🇱**

```sql
-- ✅ נכון:
OfficeName NVARCHAR(100)  -- "משרד חקירות עכו"

-- ❌ לא יעבוד:
OfficeName VARCHAR(100)   -- "????? ?????? ???"
```

---

## 7. שאילתות לדוגמה

### שאילתה 1: כל הקריאות של משרד ספציפי
```sql
SELECT 
    o.OfficeID,
    o.OfficeName,
    o.Phone,
    s.CallID,
    s.Description,
    s.Status,
    s.CreatedAt
FROM Offices o
INNER JOIN ServiceCalls s ON o.OfficeID = s.OfficeID
WHERE o.OfficeID = 1;
```

### שאילתה 2: כל המשרדים + כמות קריאות פתוחות
```sql
SELECT 
    o.OfficeID,
    o.OfficeName,
    o.Phone,
    COUNT(CASE WHEN s.Status = 'פתוח' THEN 1 END) AS OpenCallsCount
FROM Offices o
LEFT JOIN ServiceCalls s ON o.OfficeID = s.OfficeID
GROUP BY o.OfficeID, o.OfficeName, o.Phone;
```

---

## 8. סיכום טכני

| רכיב | מספר | הערות |
|------|------|-------|
| טבלאות | 4 | Offices, FieldOffices, ManagementOffices, ServiceCalls |
| קשרים | 3 | 2×(1:0..1) + 1×(1:N) |
| PK | 4 | כולם Identity מלבד FieldOffices/ManagementOffices |
| FK | 3 | כולם עם Cascade Delete |
| אינדקסים | 3 | לשיפור ביצועים |
