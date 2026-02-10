# הוראות הרצה - מערכת ניהול קריאות שירות

## שלב 1: יצירת בסיס הנתונים

### ב-Visual Studio:
1. פתח **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. ודא ש-**Default project** הוא `DAL`
3. הרץ:
```powershell
Update-Database
```

### ב-Terminal:
```bash
dotnet ef database update --project DAL --startup-project ServiceCallManagementSystem
```

---

## שלב 2: הרצת האפליקציה

### ב-Visual Studio:
- לחץ **F5** או **Ctrl+F5**

### ב-Terminal:
```bash
cd "h:\Practicum test\ServiceCallManagementSystem"
dotnet run --project ServiceCallManagementSystem
```

---

## שלב 3: בדיקת ה-API

### דרך Swagger:
1. לאחר הרצה, הדפדפן ייפתח אוטומטית ל-Swagger UI
2. או גש ל: `https://localhost:xxxx/swagger`

### API Endpoints זמינים:

#### משרדים:
- `GET /api/offices` - כל המשרדים
- `GET /api/offices/{id}` - משרד ספציפי
- `GET /api/offices/{id}/with-service-calls` - משרד + קריאות שירות
- `GET /api/offices/with-open-calls-count` - משרדים + ספירת קריאות פתוחות
- `POST /api/offices` - יצירת משרד

#### קריאות שירות:
- `GET /api/servicecalls` - כל הקריאות
- `GET /api/servicecalls/office/{officeId}` - קריאות לפי משרד
- `POST /api/servicecalls` - יצירת קריאה
- `PATCH /api/servicecalls/{id}/status` - עדכון סטטוס

---

## שלב 4: הוספת נתונים לבדיקה

### יצירת משרד:
```http
POST https://localhost:xxxx/api/offices
Content-Type: application/json

{
  "officeName": "משרד מיסוי ירושלים",
  "phone": "02-1234567",
  "officeType": "Field"
}
```

### יצירת קריאת שירות:
```http
POST https://localhost:xxxx/api/servicecalls
Content-Type: application/json

{
  "officeID": 1,
  "description": "מקלדת לא עובדת"
}
```

---

## בדיקת תקינות:

✅ Build עובר ללא שגיאות  
✅ בסיס נתונים נוצר  
✅ Swagger UI נטען  
✅ API מחזיר תגובות בפורמט camelCase  
✅ Validation עובד (נסה מספר טלפון לא תקין)  
✅ Calculated Field (urgencyLevel) מחושב נכון  

---

## פתרון בעיות:

### שגיאת Connection String:
- ודא ש-SQL Server LocalDB מותקן
- בדוק ב-Program.cs את ה-ConnectionString

### שגיאת Migration:
```bash
# מחיקת Migrations קיימים
dotnet ef migrations remove --project DAL --startup-project ServiceCallManagementSystem

# יצירה מחדש
dotnet ef migrations add InitialCreate --project DAL --startup-project ServiceCallManagementSystem
dotnet ef database update --project DAL --startup-project ServiceCallManagementSystem
```

### Port כבר בשימוש:
- שנה את ה-Port ב-`launchSettings.json`
