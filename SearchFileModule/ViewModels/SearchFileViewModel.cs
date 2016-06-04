﻿using Microsoft.Practices.Unity;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using PropertyChanged;
using SearchFile.Messaging;
using SearchFile.Messaging.FileFilters;
using SearchFile.Models;
using SearchFileModule.Properties;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.ViewModels
{
    [ImplementPropertyChanged]
    public class SearchFileViewModel : BindableBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CollectionViewSource resultsViewSource;

        [Dependency]
        public SearchFile.Models.Condition Condition { get; set; }

        public Searcher Searcher { get; }
        public ICollectionView ResultsView => this.resultsViewSource.View;
        public string Status { get; private set; }

        public DelegateCommand ChooseFolderCommand { get; }
        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ClearResultsCommand { get; }
        public DelegateCommand SaveResultsCommand { get; }
        public DelegateCommand CopyResultsCommand { get; }
        public DelegateCommand<string> SortResultsCommand { get; }

        public InteractionRequest<Notification> ChooseFolderRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ExceptionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> SaveFileRequest { get; } = new InteractionRequest<Notification>();

        public SearchFileViewModel()
        {
            this.ChooseFolderCommand = new DelegateCommand(this.ChooseFolder);
            this.SearchCommand = new DelegateCommand(this.Search);
            this.ClearResultsCommand = new DelegateCommand(this.ClearResults);
            this.SaveResultsCommand = new DelegateCommand(this.SaveResults);
            this.CopyResultsCommand = new DelegateCommand(this.CopyResults);
            this.SortResultsCommand = new DelegateCommand<string>(this.SortResults);
        }

        [InjectionConstructor]
        public SearchFileViewModel(Searcher searcher) : this()
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            PropertyChangedEventManager.AddHandler(searcher, this.SearchingDirectoryChanged, nameof(searcher.SearchingDirectory));
            this.resultsViewSource = new CollectionViewSource() { Source = searcher.Results };
            this.Searcher = searcher;
        }

        private void SearchingDirectoryChanged(object sender, PropertyChangedEventArgs e)
        {
            var searcher = (Searcher)sender;
            var directory = searcher.SearchingDirectory;

            if (directory == null)
            {
                this.Status = string.Format(Resources.SearchingResultMessage, searcher.Results.Count);
            }
            else
            {
                this.Status = string.Format(Resources.SearchingDirectoryMessage, directory);
            }
        }

        private void ChooseFolder()
        {
            this.ChooseFolderRequest.Raise(new Notification()
            {
                Content = new ChooseFolderMessage()
                {
                    Path = this.Condition.TargetDirectory
                }
            }, n => this.Condition.TargetDirectory = ((ChooseFolderMessage)n.Content).Path);
        }

        private async void Search()
        {
            try
            {
                if (this.Searcher.IsSearching)
                {
                    this.Searcher.Cancel();
                }
                else
                {
                    await this.Searcher.Search(this.Condition);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
                this.Status = Resources.SearchingErrorMessage;
            }
        }

        private void ClearResults()
        {
            this.Searcher.Clear();
            this.resultsViewSource.SortDescriptions.Clear();
            this.Status = Resources.ClearResultsMessage;
        }

        private void SaveResults()
        {
            try
            {
                this.SaveFileRequest.Raise(new Notification()
                {
                    Content = new SaveFileMessage()
                    {
                        Filters = { new CsvFileFilter(), new TextFileFilter(), new AllFileFilter() }
                    }
                }, n => this.Searcher.Save(((SaveFileMessage)n.Content).Path));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                this.ExceptionRequest.Raise(new Notification() { Content = ex });
            }
        }

        private void CopyResults()
        {
            var sb = new StringBuilder();

            foreach (var result in this.Searcher.Results)
            {
                sb.AppendLine(result.FilePath);
            }

            Clipboard.SetText(sb.ToString());
            this.Status = string.Format(Resources.CopyFileNameMessage, this.Searcher.Results.Count);
        }

        private void SortResults(string propertyName)
        {
            var direction = (from sd in this.resultsViewSource.SortDescriptions
                             where sd.PropertyName == propertyName
                             select sd.Direction).DefaultIfEmpty(ListSortDirection.Descending).First();

            this.resultsViewSource.SortDescriptions.Clear();
            this.resultsViewSource.SortDescriptions.Add(new SortDescription(propertyName,
                direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending));
        }
    }
}
