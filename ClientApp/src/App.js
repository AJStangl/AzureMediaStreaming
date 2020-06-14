import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {VideoPlayer} from "./components/VideoPlayer";
import VideoUpload from "./components/VideoUpload";
import './custom.css'
import ErrorBoundary from "./components/ErrorBoundary";


export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <ErrorBoundary>
                    <Route exact path='/' component={Home}/>
                    <Route path='/fetch-video/:id' component={VideoPlayer} exact={true}/>
                    <Route path='/upload-video' component={VideoUpload}/>
                </ErrorBoundary>
            </Layout>
        );
    }
}
