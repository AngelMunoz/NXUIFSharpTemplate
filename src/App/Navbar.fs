module Sample.Navbar

open Avalonia
open Avalonia.Controls

open NXUI.FSharp.Extensions

open Navs



let view (router: IRouter<Control>) =
  StackPanel()
    .DockTop()
    .OrientationHorizontal()
    .spacing(4.)
    .margin(Thickness(4., 8.))
    .children(
      DockPanel()
        .lastChildFill(true)
        .HorizontalAlignmentLeft()
        .children(
          Button()
            .content("Counter")
            .OnClickHandler(fun _ _ ->
              router.Navigate("/counter")
              |> Async.AwaitTask
              |> Async.Ignore
              |> Async.StartImmediate
            )
            .DockLeft(),
          Button()
            .content("To-do's")
            .OnClickHandler(fun _ _ ->
              router.Navigate("/todos")
              |> Async.AwaitTask
              |> Async.Ignore
              |> Async.StartImmediate
            )
            .DockLeft()
        )
    )
