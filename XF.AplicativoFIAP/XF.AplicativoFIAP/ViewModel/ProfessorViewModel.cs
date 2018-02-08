using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.AplicativoFIAP.Model;
using XF.AplicativoFIAP.View;

namespace XF.AplicativoFIAP.ViewModel
{
    public class ProfessorViewModel
    {
        #region Propriedades
        public IEnumerable<Professor> BkpListForSearch { get; set; }
        public Professor Professor { get; set; }
        public ObservableCollection<Professor> Professores { get; set; }
        public ICommand OnNovoCMD { get; private set; }
        public ICommand OnEditarCMD { get; private set; }
        public ICommand OnRemoverCMD { get; private set; }
        public ICommand OnSalvarCMD { get; private set; }
        public ICommand OnCancelarCMD { get; private set; }

        private string professorPesquisado;
        public string ProfessorPesquisado
        {
            get { return professorPesquisado ?? ""; }
            set
            {
                professorPesquisado = value;
                PesquisarProfessor();
            }
        }
        #endregion

        public ProfessorViewModel()
        {
            BkpListForSearch = new List<Professor>();
            Professor = new Professor();
            Professores = new ObservableCollection<Professor>();
            OnNovoCMD = new Command(OnNovo);
            OnEditarCMD = new Command<Professor>(OnEditar);
            OnRemoverCMD = new Command<Professor>(OnRemover);
            OnSalvarCMD = new Command(OnSalvar);
            OnCancelarCMD = new Command(OnCancelar);
        }

        public async Task CarregarProfessores()
        {
            BkpListForSearch = await ProfessorRepository.GetProfessoresSqlAzureAsync();
            PesquisarProfessor();
        }

        private void OnNovo()
        {
            App.Current.MainPage.Navigation.PushAsync(new NovoProfessorView() { BindingContext = App.ProfessorVM });
        }

        private void PesquisarProfessor()
        {
            var professores = BkpListForSearch.Where(prof => prof.Nome.ToLower().Contains(ProfessorPesquisado.Trim().ToLower()));
            Professores.Clear();
            foreach (var professor in professores)
                Professores.Add(professor);
        }

        private void OnEditar(Professor professor)
        {
            Professor = professor;
            App.Current.MainPage.Navigation.PushAsync(new NovoProfessorView() { BindingContext = App.ProfessorVM });
        }

        private async void OnRemover(Professor professor)
        {
            await ProfessorRepository.DeleteProfessorSqlAzureAsync(professor.Id.ToString());
            await CarregarProfessores();
        }

        private async void OnSalvar()
        {
            if (string.IsNullOrWhiteSpace(Professor.Nome) || string.IsNullOrWhiteSpace(Professor.Titulo))
                await App.Current.MainPage.DisplayAlert("Cadastro inválido!", "Preencha todos os campos.", "OK");
            else
            {
                var professor = Professor;
                Limpar();

                await ProfessorRepository.PostProfessorSqlAzureAsync(professor);
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void OnCancelar()
        {
            Limpar();
            App.Current.MainPage.Navigation.PopAsync();
        }

        private void Limpar()
        {
            Professor = new Professor();
        }
    }
}
