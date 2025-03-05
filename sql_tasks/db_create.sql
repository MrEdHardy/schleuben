CREATE TABLE Person(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    lastName TEXT NOT NULL,
    firstName TEXT NOT NULL,
    birthDate Text
);

CREATE Table Address(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    postCode TEXT NOT NULL,
    street TEXT NOT NULL,
    houseNumber TEXT NOT NULL,
    city TEXT NOT NULL,
    additionalInfo TEXT,
    persId INTEGER NOT NULL,
    FOREIGN KEY(persId) REFERENCES Person(id) ON DELETE RESTRICT
);

CREATE Table TelephoneConnection(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    telephoneNumber TEXT NOT NULL,
    persId INTEGER NOT NULL,
    FOREIGN KEY(persId) REFERENCES Person(id) ON DELETE RESTRICT
);

INSERT INTO "Person" ("id", "firstName", "lastName", "birthDate") VALUES (1, 'Max', 'Mustermann', '1990-01-01'); 
INSERT INTO "Person" ("id", "firstName", "lastName", "birthDate") VALUES (2, 'Peter', 'Lustig', '1945-01-01')

INSERT INTO "Address" ("id", "street", "houseNumber", "city", "postCode", "persId") VALUES (1, 'Musterstraße', '1', 'Musterstadt', '12345', 1); 
INSERT INTO "Address" ("id", "street", "houseNumber", "city", "postCode", "persId") VALUES (2, 'Lustigstraße', '2', 'Lustigstadt', '54321', 2);

INSERT INTO "TelephoneConnection" ("id", "telephoneNumber", "persId") VALUES (1, '0123456789', 1); 
INSERT INTO "TelephoneConnection" ("id", "telephoneNumber", "persId") VALUES (2, '9876543210', 2);
