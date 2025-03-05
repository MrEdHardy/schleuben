# Wie viele Personendatensätze sind vorhanden?
SELECT Count(*) FROM Person;

# Wie viele Personen wohnen in Dresden?
SELECT Count(p.id) FROM Person p
INNER JOIN Address a ON a.persId = p.id
WHERE a.city = 'Dresden';

# Wie viele Personen haben mehr als eine Telefonnummer?
SELECT Count(p.id) FROM Person p
INNER JOIN TelephoneConnection tc ON tc.persId = p.id
GROUP BY p.id
HAVING Count(tc.telephoneNumber) > 1;