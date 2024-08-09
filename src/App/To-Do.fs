module Sample.Todo

open Avalonia.Controls
open Avalonia.Controls.Templates

open NXUI.FSharp.Extensions
open Navs.Avalonia

open FSharp.Data.Adaptive
open System
open Avalonia

type Todo = {
  id: Guid
  title: string
  isDone: bool
}

type TodoService =
  abstract todos: aval<Todo list>
  abstract addTodo: todo: Todo -> unit
  abstract deleteTodo: todo: Todo -> unit
  abstract updatetodo: todo: Todo -> unit

module TodoService =
  let create () =
    let todoList = cval<Todo list> []

    { new TodoService with
        member _.todos = todoList

        member _.addTodo todo = todoList.setValue(fun v -> todo :: v)

        member _.deleteTodo todo =
          todoList.setValue(fun existing -> [
            for e in existing do
              if e.id = todo.id then () else e
          ])

        member _.updatetodo todo =
          todoList.setValue(fun existing -> [
            for e in existing -> if e.id = todo.id then todo else e
          ])
    }


let todoForm (todoService: TodoService) =
  let isDone = cval false
  let title = cval ""

  let onAddTodo _ _ =
    let newTodo = {
      id = Guid.NewGuid()
      isDone = isDone.Value
      title = title.Value
    }

    todoService.addTodo newTodo
    isDone.setValue(false)
    title.setValue("")

  StackPanel()
    .margin(Thickness(0, 8.))
    .spacing(4.)
    .DockTop()
    .OrientationHorizontal()
    .children(
      CheckBox().isChecked(isDone |> CVal.toBinding),
      TextBox().watermark("What's on your mind?").text(title |> CVal.toBinding),
      Button()
        .content("Add")
        .isEnabled(
          title
          |> AVal.map(fun value -> not(String.IsNullOrWhiteSpace value))
          |> AVal.toBinding
        )
        .OnClickHandler(onAddTodo)
    )

let existingTodoForm (todoService: TodoService) =
  Func<_, _, _>(fun todo _ ->
    let onisDoneChanged todo =
      fun (checkbox: CheckBox) _ ->
        todoService.updatetodo(
          {
            todo with
                isDone =
                  checkbox.IsChecked
                  |> Option.ofNullable
                  |> Option.defaultValue false
          }
        )

    StackPanel()
      .spacing(8.)
      .OrientationHorizontal()
      .children(
        CheckBox()
          .isChecked(todo.isDone)
          .OnIsCheckedChangedHandler(onisDoneChanged todo),
        TextBox()
          .text(todo.title)
          .OnTextChangedHandler(
            (fun textBox args ->
              todoService.updatetodo { todo with title = textBox.Text }
            )
          ),
        Button()
          .content("Remove")
          .OnClickHandler(fun _ _ -> todoService.deleteTodo todo)
      )
    :> Control
  )
  |> FuncDataTemplate<Todo>


let view (todoService: TodoService) _ _ =

  DockPanel()
    .lastChildFill(true)
    .children(
      todoForm todoService,
      ScrollViewer()
        .DockBottom()
        .content(
          ItemsControl()
            .itemTemplate(existingTodoForm todoService)
            .itemsSource(todoService.todos |> AVal.toBinding)
        )
    )
  :> Control
