open System.Reactive.Subjects

open Avalonia
open Avalonia.Data
open Avalonia.Controls
open Avalonia.Controls.Templates

open NXUI.Extensions
open NXUI.FSharp.Extensions


open Navs
open Navs.Router
open Navs.Avalonia

open FSharp.Data.Adaptive
open UrlTemplates.RouteMatcher
open System

type Todo = {
  id: Guid
  title: string
  isDone: bool
}

let counter _ _ =

  let counter = cval 0
  let label = counter |> AVal.map(fun v -> $"You clicked %i{v} times")

  StackPanel()
    .children(
      Button()
        .content("Click me!")
        .OnClickHandler(fun _ _ -> counter.setValue(fun v -> v + 1)),
      TextBlock().text(label |> AVal.toBinding)
    )

let todos
  (
    todoList: aval<Todo list>,
    onTodoAdded: Todo -> unit,
    onTodoUpdated: Todo -> unit
  )
  _
  _
  =
  let newTodoDone = cval false
  let newTodoText = cval ""

  DockPanel()
    .lastChildFill(true)
    .children(
      StackPanel()
        .spacing(4.)
        .OrientationHorizontal()
        .children(
          CheckBox().isChecked(newTodoDone |> CVal.toBinding),
          TextBox()
            .watermark("What's left?")
            .text(newTodoText |> CVal.toBinding),
          Button()
            .content("Add")
            .OnClickHandler(fun _ _ ->
              let newTodo = {
                id = Guid.NewGuid()
                isDone = newTodoDone.Value
                title = newTodoText.Value
              }

              onTodoAdded newTodo
            )
        ),
      ScrollViewer()
        .content(
          ItemsRepeater()
            .itemTemplate(
              FuncDataTemplate<Todo>(fun todo _ ->
                Grid()
                  .columnDefinitions("*,*")
                  .rowDefinitions("*,*")
                  .children(
                    CheckBox()
                      .OnIsCheckedChangedHandler(fun checkbox _ ->
                        onTodoUpdated(
                          {
                            todo with
                                isDone =
                                  checkbox.IsChecked
                                  |> Option.ofNullable
                                  |> Option.defaultValue false
                          }
                        )
                      ),
                    TextBlock().row(0).column(1),
                    TextBlock().row(1).column(0),
                    TextBlock().row(1).column(1)
                  )
              )
            )
            .itemsSource(todoList)
        )
    )

let todoDetail (todoList: aval<Todo list>) (ctx: RouteContext) _ =
  let content =
    match ctx.urlMatch |> UrlMatch.getFromParams "id" with
    | ValueNone -> AVal.constant(TextBlock().text("Not Found"))
    | ValueSome value ->
      todoList
      |> AVal.map(fun values ->
        let todo = values |> List.tryFind(fun v -> v.id = value)
        TextBlock().text("")
      )

  UserControl().content(content |> AVal.toBinding)

let getRoutes () =
  let todoList = cval<Todo list> []
  let onTodoAdded todo = todoList.setValue(fun v -> todo :: v)

  let onTodoUpdated todo =
    todoList.setValue(fun existing -> [
      for e in existing -> if e.id = todo.id then todo else e
    ])

  [
    Route.define("counter", "/counter", counter)
    Route.define("todos", "/todos", todos(todoList, onTodoAdded, onTodoUpdated))
    Route.define("todo-detail", "/todos/:id<guid>", todoDetail todoList)
  ]

let navbar (router: IRouter<Control>) =
  let left =
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

  let right = DockPanel().lastChildFill(true).HorizontalAlignmentRight()

  let center = DockPanel().lastChildFill(true).HorizontalAlignmentCenter()

  StackPanel()
    .DockTop()
    .OrientationHorizontal()
    .spacing(4.)
    .children(left, center, right)
    .margin(Thickness(4., 8.))

let view () : Window =

  let router = AvaloniaRouter(getRoutes())

  Window()
    .title("NXUI and F#")
    .width(300)
    .height(300)
    .content(
      DockPanel()
        .lastChildFill(true)
        .children(
          navbar router,
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
