-- בדיקת נתונים בבסיס הנתונים
-- הרץ ב-SQL Server Management Studio או ב-Visual Studio (SQL Server Object Explorer)

USE ServiceCallManagementDB;
GO

-- בדיקת משרדים
SELECT * FROM Offices;

-- בדיקת קריאות שירות
SELECT * FROM ServiceCalls;

-- בדיקת משרדי שטח
SELECT * FROM FieldOffices;

-- בדיקת משרדי ניהול
SELECT * FROM ManagementOffices;

-- הוספת נתונים לדוגמה רק אם הטבלאות ריקות:

-- בדיקה והוספת משרדים
IF (SELECT COUNT(*) FROM Offices) = 0
BEGIN
    INSERT INTO Offices (OfficeName, Phone, OfficeType)
    VALUES 
        (N'משרד מיסוי ירושלים', '02-6543210', 'Field'),
        (N'משרד מיסוי תל אביב', '03-7654321', 'Field'),
        (N'משרד ניהול מרכז', '03-9876543', 'Management');
    
    SELECT 'נוספו 3 משרדים' AS Result;
END
ELSE
BEGIN
    SELECT 'טבלת Offices כבר מכילה נתונים' AS Result;
END

-- בדיקה והוספת קריאות שירות
IF (SELECT COUNT(*) FROM ServiceCalls) = 0
BEGIN
    INSERT INTO ServiceCalls (OfficeID, Description, Status, CreatedAt)
    VALUES 
        (1, N'מקלדת לא עובדת', N'פתוח', GETDATE()),
        (1, N'מדפסת תקועה', N'בטיפול', GETDATE()),
        (2, N'בעיה במסך', N'טופל', GETDATE());
    
    SELECT 'נוספו 3 קריאות שירות' AS Result;
END
ELSE
BEGIN
    SELECT 'טבלת ServiceCalls כבר מכילה נתונים' AS Result;
END
