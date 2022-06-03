using Alura.ListaLeitura.App.HTML;
using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App.Logica
{
    public class LivrosController
    {
        public static string CarregaLista(IEnumerable<Livro> livros)
        {
            var conteudoArquivo = HtmlUtils.CarregaArquivoHTML("lista");

            foreach (var livro in livros)
            {
                conteudoArquivo = conteudoArquivo
                    .Replace("#NOVO-ITEM#", $"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM#");
            }

            return conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", "");
        }

        public IActionResult ParaLer()
        {
            var _repo = new LivroRepositorioCSV();
            //var html = CarregaLista(_repo.ParaLer.Livros);
            var html = new ViewResult { ViewName = "lista" };

            return html;
        }

        public IActionResult Lendo()
        {
            var _repo = new LivroRepositorioCSV();
            //var html = CarregaLista(_repo.Lendo.Livros);
            var html = new ViewResult { ViewName = "lista" };

            return html;
        }

        public IActionResult Lidos()
        {
            var _repo = new LivroRepositorioCSV();
            //var html = CarregaLista(_repo.Lidos.Livros);
            var html = new ViewResult { ViewName = "lista" };

            return html;
        }

        public string Detalhes(int id)
        {
            var repo = new LivroRepositorioCSV();
            var livro = repo.Todos.First(l => l.Id == id);

            return livro.Detalhes();
        }
    }
}
