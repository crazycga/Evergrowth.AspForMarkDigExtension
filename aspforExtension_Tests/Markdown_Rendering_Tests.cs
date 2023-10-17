using Markdig;
using Evergrowth.AspForMarkDigExtension;
using TestingProject.Database.FormObjects;
using TestingProject.Models;
using System.Diagnostics;
using Evergrowth.AspForMarkDigExtension.Enums;
using Evergrowth.AspForMarkDigExtension.Support;

namespace Evergrowth.AspForMarkDigExtension_Tests;

public class Rendering_Tests
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
    public class Markdown_Rendering_Tests
    {
        public BasicForm_class newFormPrototype;

        public Markdown_Rendering_Tests()
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
        public void Basic_Functionality()
        {
            MarkdownPipeline pipeline;
            string html;
            string markdown;

            markdown = "this is !ASP-FOR[formData.tbRandomName] inserted here." + Environment.NewLine + "this is !ASP-FOR[MajorVersion] inserted here." + Environment.NewLine;
            markdown += "this is !ASP-FOR[formData.tbRandomRate] that is here." + Environment.NewLine + "and this is !ASP-FOR[formData.cbCheckCondition1] as it stands.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);

            string ExpectedOutput = "<p>this is <input type=\"text\" id=\"formData_tbRandomName\" name=\"formData.tbRandomName\" value=\"Person McPersonFace\" readonly /> inserted here.\nthis is <input type=\"number\" id=\"MajorVersion\" name=\"MajorVersion\" value=\"1\" readonly /> inserted here.\nthis is <input type=\"number\" id=\"formData_tbRandomRate\" name=\"formData.tbRandomRate\" value=\"15.99\" readonly /> that is here.\nand this is <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" checked readonly /> as it stands.</p>\n";

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_No_Readonly()
        {
            MarkdownPipeline pipeline;
            string html;
            string markdown;

            markdown = "this is !ASP-FOR[formData.tbRandomName] inserted here." + Environment.NewLine + "this is !ASP-FOR[MajorVersion] inserted here." + Environment.NewLine;
            markdown += "this is !ASP-FOR[formData.tbRandomRate] that is here." + Environment.NewLine + "and this is !ASP-FOR[formData.cbCheckCondition1] as it stands.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);

            string ExpectedOutput = "<p>this is <input type=\"text\" id=\"formData_tbRandomName\" name=\"formData.tbRandomName\" value=\"Person McPersonFace\" /> inserted here.\nthis is <input type=\"number\" id=\"MajorVersion\" name=\"MajorVersion\" value=\"1\" /> inserted here.\nthis is <input type=\"number\" id=\"formData_tbRandomRate\" name=\"formData.tbRandomRate\" value=\"15.99\" /> that is here.\nand this is <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" checked /> as it stands.</p>\n";

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_No_Values()
        {
            MarkdownPipeline pipeline;
            string html;
            string markdown;

            markdown = "this is !ASP-FOR[formData.tbRandomName] inserted here." + Environment.NewLine + "this is !ASP-FOR[MajorVersion] inserted here." + Environment.NewLine;
            markdown += "this is !ASP-FOR[formData.tbRandomRate] that is here." + Environment.NewLine + "and this is !ASP-FOR[formData.cbCheckCondition1] as it stands.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);

            string ExpectedOutput = "<p>this is <input type=\"text\" id=\"formData_tbRandomName\" name=\"formData.tbRandomName\"  /> inserted here.\nthis is <input type=\"number\" id=\"MajorVersion\" name=\"MajorVersion\"  /> inserted here.\nthis is <input type=\"number\" id=\"formData_tbRandomRate\" name=\"formData.tbRandomRate\"  /> that is here.\nand this is <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" /> as it stands.</p>\n";

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_With_Warning_On_Missing_Field()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "this is !ASP-FOR[formData.DoesntExist] inserted here." + Environment.NewLine + "this is !ASP-FOR[MajorVersion] inserted here." + Environment.NewLine;
            markdown += "this is !ASP-FOR[formData.tbRandomRate] that is here." + Environment.NewLine + "and this is !ASP-FOR[formData.cbCheckCondition1] as it stands.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<p>this is <input type=\"text\" id=\"formData_DoesntExist\" name=\"formData.DoesntExist\" value=\"Field formData.DoesntExist not found.\" readonly /> inserted here.\nthis is <input type=\"number\" id=\"MajorVersion\" name=\"MajorVersion\" value=\"1\" readonly /> inserted here.\nthis is <input type=\"number\" id=\"formData_tbRandomRate\" name=\"formData.tbRandomRate\" value=\"15.99\" readonly /> that is here.\nand this is <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" checked readonly /> as it stands.</p>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_With_Error_On_Missing_Field()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "this is !ASP-FOR[formData.DoesntExist] inserted here." + Environment.NewLine + "this is !ASP-FOR[MajorVersion] inserted here." + Environment.NewLine;
            markdown += "this is !ASP-FOR[formData.tbRandomRate] that is here." + Environment.NewLine + "and this is !ASP-FOR[formData.cbCheckCondition1] as it stands.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowError;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            var ex = Assert.Throws<ArgumentNullException>(() => Markdown.ToHtml(markdown, pipeline));

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'modelReference')"));
        }

        [Test]
        public void Basic_Functionality_With_No_Tags_In_Markdown()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is nothing inserted here.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is nothing inserted here.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_With_Random_Exclamations_In_Markdown()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing i!!nserted !ASPhere.";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing i!!nserted !ASPhere.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_With_No_Blank_After_Tag()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.tbRandomName].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"text\" id=\"formData_tbRandomName\" name=\"formData.tbRandomName\" value=\"Person McPersonFace\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Basic_Functionality_For_Checkbox()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbCheckCondition1].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" checked readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Set_Restriction_Very_Low_and_Not_Find_Anything()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbCheckCondition1].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.NullHandling = NullHandling.ShowWarning;
            options.MaxReferenceLength = 2;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing rmData.cbCheckCondition1].</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Against_Null_Checkbox_Type_As_Unchecked()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbCheckCondition2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbCheckCondition2\" name=\"formData.cbCheckCondition2\" value=\"True\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Against_Checkbox_Type_As_Checked()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbCheckCondition1].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbCheckCondition1\" name=\"formData.cbCheckCondition1\" value=\"True\" checked readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Against_Checkbox_Type_As_Unset_In_Object()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbCheckCondition2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbCheckCondition2\" name=\"formData.cbCheckCondition2\" value=\"True\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Date_Override_as_DateTime_when_Null()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.dtSubName2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.DateTimeOverride = DateTimeOverride.AsDateTimeLocal;
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"datetime-local\" id=\"formData_dtSubName2\" name=\"formData.dtSubName2\" value=\"\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Date_when_Null()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.dtSubName2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            Debug.WriteLine(newFormPrototype.formData.dtSubName2.ToString());
            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"date\" id=\"formData_dtSubName2\" name=\"formData.dtSubName2\" value=\"\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Date_when_Date_Only_Entered()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.dtSubName2].";

            newFormPrototype.formData.dtSubName2 = DateTime.Parse("2023-10-14");

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"date\" id=\"formData_dtSubName2\" name=\"formData.dtSubName2\" value=\"2023-10-14\" readonly />.</h2>\n";
            RefreshTestObject();
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Date_when_Time_Only_Entered()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.dtSubName2].";

            newFormPrototype.formData.dtSubName2 = Convert.ToDateTime(DateTime.Parse("19:34"));

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.DateTimeOverride = DateTimeOverride.AsTime;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"time\" id=\"formData_dtSubName2\" name=\"formData.dtSubName2\" value=\"19:34\" readonly />.</h2>\n";
            RefreshTestObject();
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Date_when_DateTime_Entered()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.dtSubName2].";

            newFormPrototype.formData.dtSubName2 = DateTime.Parse("2023-10-14T19:34");

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.PopulateTagsWithModelData = true;
            options.DateTimeOverride = DateTimeOverride.AsDateTimeLocal;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"datetime-local\" id=\"formData_dtSubName2\" name=\"formData.dtSubName2\" value=\"2023-10-14T19:34\" readonly />.</h2>\n";
            RefreshTestObject();
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Check_Ignored_Value_Is_Ignored_When_Empty()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing ## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials1]..";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing ## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials1]..</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Check_Ignored_Value_Is_Ignored_When_Data_Present()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            newFormPrototype.formData.tbAcceptanceInitials1 = "TEST";

            markdown = "## this is n!othing ## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials1]..";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing ## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials1]..</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Readonly_Attribute_on_Text_With_Data()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            newFormPrototype.formData.tbAcceptanceInitials2 = "Sample Data Added";
            markdown = "## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"text\" id=\"formData_tbAcceptanceInitials2\" name=\"formData.tbAcceptanceInitials2\" value=\"Sample Data Added\" readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Readonly_Attribute_on_Text_Without_Data()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.tbAcceptanceInitials2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"text\" id=\"formData_tbAcceptanceInitials2\" name=\"formData.tbAcceptanceInitials2\" value=\"\" readonly />.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Readonly_Attribute_on_Checkbox_With_Data()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            newFormPrototype.formData.cbYesNo3 = true;
            markdown = "## this is n!othing !ASP-FOR[formData.cbYesNo3].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbYesNo3\" name=\"formData.cbYesNo3\" value=\"Excluded\" checked readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Readonly_Attribute_on_Checkbox_Without_Data()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbYesNo3].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbYesNo3\" name=\"formData.cbYesNo3\" value=\"Excluded\" readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Checked_Value_Attribute_Working_Unchecked()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.cbYesNo2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbYesNo2\" name=\"formData.cbYesNo2\" value=\"True\" readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Default_Checked_Value_Attribute_Working_Checked()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            newFormPrototype.formData.cbYesNo2 = true;
            markdown = "## this is n!othing !ASP-FOR[formData.cbYesNo2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbYesNo2\" name=\"formData.cbYesNo2\" value=\"True\" checked readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Test_Default_Checked_Value_Attribute_Working_Unchecked()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            newFormPrototype.formData.cbYesNo2 = false;
            markdown = "## this is n!othing !ASP-FOR[formData.cbYesNo2].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"checkbox\" id=\"formData_cbYesNo2\" name=\"formData.cbYesNo2\" value=\"True\" readonly />.</h2>\n";
            RefreshTestObject();

            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }

        [Test]
        public void Check_DateTime_Attribute_Override_Renders_Correctly()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            AspForUtilities.AspForObjectInfo_struct? objInfo;
            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);

            markdown = "This is! a check !ASP-FOR[formData.dtSubName1] field";

            newFormPrototype.formData.dtSubName1 = DateTime.Parse("2021-10-20 19:34");

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            RefreshTestObject();

            string Expected = "<p>This is! a check <input type=\"time\" id=\"formData_dtSubName1\" name=\"formData.dtSubName1\" value=\"19:34\" readonly /> field</p>\n";
            Assert.That(html, Is.EqualTo(Expected));
        }

        [Test]
        public void Test_Required_Attribute_on_Text_Without_Data()
        {
            MarkdownPipeline pipeline;
            string html = string.Empty;
            string markdown;

            markdown = "## this is n!othing !ASP-FOR[formData.tbName1].";

            AspForGeneratorOptions options = new AspForGeneratorOptions(newFormPrototype);
            options.DesignatePopulatedFieldsReadOnly = false;

            pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();

            html = Markdown.ToHtml(markdown, pipeline);
            string ExpectedOutput = "<h2>this is n!othing <input type=\"text\" id=\"formData_tbName1\" name=\"formData.tbName1\" value=\"\" required/>.</h2>\n";
            Assert.That(html, Is.EqualTo(ExpectedOutput));
        }
    }
}