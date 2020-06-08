import React, {Component} from 'react';

class ErrorBoundary extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            errorInfo: null
        };
    }

    componentDidCatch(error, errorInfo) {
        this.setState({
            error: error,
            errorInfo: errorInfo
        });
    }

    render() {
        if (this.state.errorInfo) {
            return (
                React.createElement("div", null,
                    React.createElement("h2", null, "Something went wrong."),
                    React.createElement("details", {
                            style: {
                                whiteSpace: 'pre-wrap'
                            }
                        },
                        this.state.error && this.state.error.toString(),
                        React.createElement("br", null),
                        this.state.errorInfo.componentStack)));
        }
        return this.props.children;
    }
}

export default ErrorBoundary