select * from trainee where;

select * from [PERSON] 
where 
(firstName like 'SINTAYEHU' and MiddleName like 'KIFLU') or (firstName like 'WONDESSEN' and MiddleName like 'DESSALEGN');

select * from TRAINEE where TraineeId in(136,562);

0922803057->(personid->136),0920207546->(personid=562)
update [Person] set Phone='0922803057' where PersonId=136;

select * from Person where CompanyId='00017986';