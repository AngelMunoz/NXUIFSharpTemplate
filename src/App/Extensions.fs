[<AutoOpen>]
module Extensions

open FSharp.Data.Adaptive
open Avalonia.Controls
open Avalonia.Controls.Templates
open System.Collections
open Avalonia.Data
open Navs.Avalonia


type ItemsRepeater with

  member this.itemTemplate(template: IDataTemplate) =
    this.ItemTemplate <- template
    this

  member this.itemsSource(items: IEnumerable) =
    this.ItemsSource <- items
    this

  member this.itemsSource(items: aval<_>) =
    let binding =
      ItemsRepeater.ItemsSourceProperty
        .Bind()
        .WithMode(BindingMode.OneWay)
        .WithPriority(BindingPriority.LocalValue)

    this[binding] <- AVal.toBinding items
    this

type Grid with

  member this.columnDefinitions(value) =
    this.ColumnDefinitions <- ColumnDefinitions(value)
    this

  member this.rowDefinitions(value) =
    this.RowDefinitions <- RowDefinitions(value)
    this
