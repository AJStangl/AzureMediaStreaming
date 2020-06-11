import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {FetchData} from './components/FetchData';
import {Counter} from './components/Counter';
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
                    <Route path='/counter' component={Counter}/>
                    <Route path='/fetch-data' component={FetchData}/>
                    <Route path='/fetch-video' component={VideoPlayer}/>
                    <Route path='/upload-video' component={VideoUpload}/>
                </ErrorBoundary>
            </Layout>
        );
    }
}
