import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {VideoPlayer} from "./components/VideoPlayer";
import VideoUpload from "./components/VideoUpload";
import './custom.css'
import ErrorBoundary from "./components/ErrorBoundary";
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import {ApplicationPaths} from './components/api-authorization/ApiAuthorizationConstants';


export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <ErrorBoundary>
                    <AuthorizeRoute exact path='/' component={Home}/>
                    <AuthorizeRoute path='/fetch-video/:id' component={VideoPlayer} exact={true}/>
                    <AuthorizeRoute path='/upload-video' component={VideoUpload}/>
                    <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes}/>
                </ErrorBoundary>
            </Layout>
        );
    }
}
