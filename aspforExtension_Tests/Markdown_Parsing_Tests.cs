using Markdig;
using Evergrowth.AspForMarkDigExtension;
using TestingProject.Database.FormObjects;
using TestingProject.Models;
using System.Diagnostics;
using Evergrowth.AspForMarkDigExtension.Enums;
using Evergrowth.AspForMarkDigExtension.Support;

namespace Evergrowth.AspForMarkDigExtension_Tests;

public class Parsing_Tests
{
    private class AspModelTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [SetUp]
    public void Setup()
    {
    }

    [TestFixture]
    public class Markdown_Parsing_Tests
    {
        public BasicForm_class newFormPrototype;

        public Markdown_Parsing_Tests()
        {
            RefreshTestObject();
        }

        public void RefreshTestObject()
        {
            newFormPrototype = new BasicForm_class();
            newFormPrototype.FunctionalFormType = FunctionalFormType.FirstForm;
            newFormPrototype.MajorVersion = 1;
            newFormPrototype.MinorVersion = 0;
            newFormPrototype.PatchVersion = 0;
            newFormPrototype.formData.tbRandomName = "Person McPersonFace";
            newFormPrototype.formData.tbRandomRate = 15.99M;
            newFormPrototype.formData.tbRateInitials1 = "JMC";
            newFormPrototype.formData.cbCheckCondition1 = true;
        }

        [Test]
        public void Check_Readonly_Attribute_Success_With_Value_Included()
        {
            newFormPrototype.formData.tbAcceptanceInitials2 = "XXX";
            AspForUtilities.AspForObjectInfo_struct? objInfo;

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            var getValue = (string)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.tbAcceptanceInitials2", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.ReadOnly, Is.True);
        }

        [Test]
        public void Check_Readonly_Attribute_Success_Without_Value_Included()
        {
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            var getValue = (string)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.tbAcceptanceInitials2", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.ReadOnly, Is.True);
        }


        [Test]
        public void Check_CheckedValue_Attribue_Functional()
        {
            newFormPrototype.formData.cbAcceptance3 = true;
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            bool getValue = (bool)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.cbAcceptance3", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.CheckedValue, Is.EqualTo("Shadows"));
            Assert.That(getValue, Is.True);
        }

        [Test]
        public void Check_Default_CheckedValue_Attribue_Functional()
        {
            newFormPrototype.formData.cbAcceptance2 = true;
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            bool getValue = (bool)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.cbAcceptance2", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.CheckedValue, Is.EqualTo("True"));
            Assert.That(getValue, Is.True);
        }

        [Test]
        public void Check_CheckedValue_Attribue_NonFunctional_On_NonBoolean()
        {
            newFormPrototype.formData.tbAcceptanceInitials3 = "Assigned";
            string CheckedValue;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            var ex = Assert.Throws<ArgumentException>(() => CheckedValue = AspForUtilities.BooleanCheckedValue(options, newFormPrototype, "formData.tbAcceptanceInitials3"));
            RefreshTestObject();

            Assert.That(ex.Message, Is.EqualTo("CheckedValue attribute is only valid on boolean type properties. (Parameter 'formData.tbAcceptanceInitials3')"));
        }

        [Test]
        public void Check_CheckedValue_Attribute_Function_On_Boolean()
        {
            newFormPrototype.formData.cbYesNo3 = true;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            var returnValue = AspForUtilities.BooleanCheckedValue(options, newFormPrototype, "formData.cbYesNo3");
            RefreshTestObject();

            Assert.That(returnValue, Is.EqualTo("Excluded"));
        }

        [Test]
        public void Check_Default_CheckedValue_Against_Unused_Checkbox()
        {
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            bool getValue = Convert.ToBoolean(AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.cbAcceptance2", out objInfo));
            RefreshTestObject();

            Assert.That(objInfo.CheckedValue, Is.EqualTo("True"));
        }

        [Test]
        public void Check_CheckedValue_Against_Unused_Checkbox()
        {
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            bool getValue = Convert.ToBoolean(AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.cbAcceptance3", out objInfo));
            RefreshTestObject();

            Assert.That(objInfo.CheckedValue, Is.EqualTo("Shadows"));
        }

        [Test]
        public void Test_Multiple_Attributes_on_Single_Property()
        {
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            bool getValue = Convert.ToBoolean(AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.cbYesNo3", out objInfo));
            RefreshTestObject();

            Assert.That(objInfo.CheckedValue, Is.EqualTo("Excluded"));
            Assert.That(objInfo.ReadOnly, Is.True);
        }

        [Test]
        public void Check_DateTime_Override_Local_Attribute()
        {
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            newFormPrototype.formData.dtSubName1 = DateTime.Parse("2021-10-20 19:34");
            DateTime getValue = Convert.ToDateTime(AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.dtSubName1", out objInfo));
            RefreshTestObject();

            Assert.That(objInfo.DateTimeOverride, Is.EqualTo(DateTimeOverride.AsTime));
        }

        [Test]
        public void Test_Parsing_Object_Changed_After_Init()
        {
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype); 
            newFormPrototype.formData.tbAcceptanceInitials2 = "XXX";
            AspForUtilities.AspForObjectInfo_struct? objInfo;
            
            var getValue = (string)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.tbAcceptanceInitials2", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.ReadOnly, Is.True);
        }

        [Test]
        public void Check_RequiredInput_Attribute()
        {
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            newFormPrototype.formData.tbRateInitials1 = "XXX";
            AspForUtilities.AspForObjectInfo_struct? objInfo;

            var getValue = (string)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.tbRateInitials1", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.Required, Is.True);
        }

        [Test]
        public void Check_RequiredInput_Attribute_Default_False()
        {
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            newFormPrototype.formData.tbRandomName = "XXX";
            AspForUtilities.AspForObjectInfo_struct? objInfo;

            var getValue = (string)AspForUtilities.GetPropertyValueAndAttributes(options, newFormPrototype, "formData.tbRandomName", out objInfo);
            RefreshTestObject();

            Assert.That(objInfo.Required, Is.False);
        }
    }
}