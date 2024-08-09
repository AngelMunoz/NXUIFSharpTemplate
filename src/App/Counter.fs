module Sample.Counter

open FSharp.Data.Adaptive

open Avalonia.Controls

open NXUI.FSharp.Extensions

open Navs.Avalonia


let view _ _ =

  let counter = cval 0
  let label = counter |> AVal.map(fun v -> $"You clicked %i{v} times")

  StackPanel()
    .children(
      Button()
        .content("Click me!")
        .OnClickHandler(fun _ _ -> counter.setValue(fun v -> v + 1)),
      TextBlock().text(label |> AVal.toBinding)
    )
  :> Control
