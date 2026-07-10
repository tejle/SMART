namespace SMART.Gui.View.TestcaseCodeGeneration
{
    using System.Windows.Media;

    using CodeBoxControl.Decorations;

    using ViewModel;

    /// <summary>
    /// Interaction logic for TestcaseCodeGenerationView.xaml
    /// </summary>
    public partial class TestcaseCodeGenerationView
    {
        private IViewModel viewModel;

        public TestcaseCodeGenerationView()
        {
            InitializeComponent();
            InitCodeBox();
            Loaded += this.TestcaseCodeGenerationView_Loaded;
        }

        void TestcaseCodeGenerationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel = DataContext as IViewModel;
            if (this.viewModel != null)
            {
                this.viewModel.ViewLoaded();
            }
        }

        void InitCodeBox()
        {
            this.codeBox.FontSize = 12;
            this.codeBox.Decorations.Clear();

            //Color most reserved words blue
            var bluekeyWords = new MultiRegexWordDecoration { Brush = new SolidColorBrush(Colors.Blue) };
            bluekeyWords.Words.AddRange(GetBlueKeyWords());
            this.codeBox.Decorations.Add(bluekeyWords);

            var dataTypes = new MultiRegexWordDecoration { Brush = new SolidColorBrush(Colors.Blue) };
            dataTypes.Words.AddRange(GetDataTypes());
            this.codeBox.Decorations.Add(dataTypes);

            var operators = new MultiStringDecoration {Brush = new SolidColorBrush(Colors.Gray)};
            operators.Strings.AddRange(GetOperators());
            this.codeBox.Decorations.Add(operators);

            //Color single line comments green
            var singleLineComment = new RegexDecoration
                                        {
                                                DecorationType = EDecorationType.TextColor,
                                                Brush = new SolidColorBrush(Colors.Green),
                                                RegexString = "//.*"
                                        };
            this.codeBox.Decorations.Add(singleLineComment);

            // Quoted text
            var quotedText = new RegexDecoration {Brush = new SolidColorBrush(Colors.Maroon), RegexString = "\".*?\"" };
            this.codeBox.Decorations.Add(quotedText);

            // Numbers
            var numbers = new RegexDecoration { Brush = new SolidColorBrush(Colors.Red), RegexString = "[0-9].*?" };
            this.codeBox.Decorations.Add(numbers);
        }

        private static string[] GetBlueKeyWords()
        {
            string[] res = { "void", "params", "event", "public", "private", "virtual", "internal", "using", "class", "namespace", "get", "set", "return" };
            return res;
        }

        private static string[] GetDataTypes()
        {
            string[] res = { "string", "int", "bool", "double", "decimal" };
            return res;
        }

        private string[] GetOperators()
        {
            string[] ops = { "=", "+", ".", ",", "-", "(", ")", "*", "<", ">" };

            return ops;

        }
    }
}
