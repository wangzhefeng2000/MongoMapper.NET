﻿using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Exceptions;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class RelationsTest
    {
    
        [Test]
        public void TestRelations()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save();

            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "XXXXX",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Childs.Add(
                new Child {ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"});

            try
            {
                p.Save();
                Assert.Fail();
            }
            catch (ValidateUpRelationException ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (ValidateUpRelationException));
                p.Country = "ES";
                p.Save();
            }

            c = MongoMapper<Country>.FindByKey("ES");
            try
            {
                c.Delete();
                Assert.Fail();
            }
            catch (ValidateDownRelationException ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (ValidateDownRelationException));
                List<Person> persons = new List<Person>();
                persons.MongoFind(C => C.Country, "ES");
                foreach (Person p2 in persons)
                {
                    p2.Country = "UK";
                    p2.Save();
                }
                c.Delete();
            }            

            List<Person> personsInUk = new List<Person>();
            personsInUk.MongoFind(C => C.Country, "UK");
            foreach (Person personInUk in personsInUk)
            {
                Assert.AreEqual(personInUk.Country, "UK");                
            }
        }
    }
}