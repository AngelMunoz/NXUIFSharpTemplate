open Avalonia
open Avalonia.Controls

open NXUI.Extensions
open NXUI.FSharp.Extensions

open Navs.Avalonia

open Sample
open Navs

let getRoutes (todoService: Todo.TodoService) = [
  Route.define("counter", "/counter", Counter.view)
  Route.define("todos", "/todos", Todo.view todoService)
]

let view () : Window =

  // use your own DI container or DI env
  let todoService = Todo.TodoService.create()
  let router: IRouter<Control> = AvaloniaRouter(getRoutes todoService)

  // Initial navigation after setting up the app
  router.Navigate("/counter")
  |> Async.AwaitTask
  |> Async.Ignore
  |> Async.StartImmediate


  Window()
    .title("NXUI and F#")
    .width(300)
    .height(300)
    .content(
      DockPanel()
        .lastChildFill(true)
        .children(
          Navbar.view router,
          RouterOutlet().router(router).margin(Thickness(4., 0.)).DockTop()
        )
    )


[<EntryPoint>]
let main argv =
  AppBuilder
    .Configure<Application>()
    .UsePlatformDetect()
    .UseFluentTheme(Styling.ThemeVariant.Dark)
    .WithApplicationName("NXUI and F#")
    .StartWithClassicDesktopLifetime(view, argv)
