# Erstellen Sie eine Abfrage, welche die Anzahl der Personen pro Ort ausgibt.
SELECT Count(DISTINCT p.id), a.city FROM PERSON p
INNER JOIN Address a ON a.persId = p.id
GROUP BY a.city;

# Erstellen Sie eine View, welche alle Personen mit deren Anschriften und Telefonnummer als eine Ergebnistabelle ausgibt.
CREATE VIEW IF NOT EXISTS personAddressTelephoneConnectionView
AS
SELECT
    p.id AS personId,
    p.lastName,
    p.firstName,
    p.birthDate,
    a.id AS addressId,
    a.postCode,
    a.city,
    a.street,
    a.houseNumber,
    tc.id AS telephoneId,
    tc.telephoneNumber
FROM Person p
INNER JOIN Address a ON a.persId = p.Id
INNER JOIN TelephoneConnection tc ON tc.persId = p.Id;

# Erstellen Sie einen Löschbefehl, welcher alle Telefonnummern löscht, welche nicht mit '0' oder '+' beginnen.
DELETE FROM TelephoneConnection tc
WHERE tc.telephoneNumber NOT LIKE '0%' AND tc.telephoneNumber NOT LIKE '+%';

# Erstellen Sie einen Befehl, um die Tabelle der Entität Person um eine Spalte zu erweitern, welche später den Namen in Großschreibung beinhaltet.
# Erstellen Sie einen Befehl, um die neu angelegte Spalte mit dem Namen der Person komplett in Großbuchstaben zu befüllen.
ALTER TABLE Person
ADD COLUMN lastNameUpper TEXT;

UPDATE Person
SET lastNameUpper = upper(lastName);