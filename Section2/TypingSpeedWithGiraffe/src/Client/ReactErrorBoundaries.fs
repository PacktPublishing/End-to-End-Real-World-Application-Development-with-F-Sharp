module ReactErrorBoundary

open Fable.Import
open Fable.Helpers.React

type [<AllowNullLiteral>] InfoComponentObject =
    abstract componentStack: string with get

type ErrorBoundaryProps =
    { Inner : React.ReactElement
      ErrorComponent : React.ReactElement
      OnError : exn * InfoComponentObject -> unit }

type ErrorBoundaryState =
    { HasErrors : bool }

// See https://reactjs.org/docs/error-boundaries.html
type ErrorBoundary (props) =
    inherit React.Component<ErrorBoundaryProps, ErrorBoundaryState> (props)
    do base.setInitState { HasErrors = false }

    override x.componentDidCatch(error, info) =
        x.props.OnError(error, info :?> InfoComponentObject)
        x.setState(fun _ _ -> { HasErrors = true })

    override x.render() =
        if x.state.HasErrors then
            x.props.ErrorComponent
        else
            x.props.Inner

let renderCatchSimple errorElement element =
    ofType<ErrorBoundary,_,_> { Inner = element; ErrorComponent = errorElement; OnError = ignore } [ ]

let renderCatchFn onError errorElement element =
    ofType<ErrorBoundary,_,_> { Inner = element; ErrorComponent = errorElement; OnError = onError } [ ]