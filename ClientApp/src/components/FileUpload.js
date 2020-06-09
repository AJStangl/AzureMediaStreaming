import React from 'react';
import {Card} from "reactstrap";
import Loader from "./Loader";

class FileUpload extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            complete: false,
            error: false,
            errorMessage: '',
            form: {
                firstName: '',
                lastName: '',
                streetName: '',
                streetNumber: '',
                file: null
            }
        };
    }

    handleChange(event) {
        event.preventDefault();
        let form = this.state.form;
        let name = event.target.name;
        form[name] = event.target.value;

        this.setState({form})
    }

    setFile(e) {
        this.setState({
            form: {
                file: e.target.files[0]
            }
        });
    }

    async handleSubmit(event) {
        event.preventDefault();
        console.log(event)
        const data = new FormData(event.target);
        data.set('file', this.state.file);
        this.setState({loading: true})
        await fetch('/media/video', {
            method: 'POST',
            body: data,
        }).then(async response => {
            if (response.ok) {
                await response.json().then(x => {
                    console.log("I am in the json promise" + x)
                    this.setState({complete: true, loading: false})
                })
                this.setState({complete: true, loading: false})
            } else {
                this.setState({
                    complete: false,
                    loading: false,
                    error: true,
                    errorMessage: response.statusText
                })
            }
        }).catch(err =>
            this.setState({
                loading: false,
                error: true,
                errorMessage: err
            }));
    }

    RenderLoading() {
        return (
            <div className={'container-fluid'}>
                <p> Uploading File...</p>
                <Loader/>
            </div>
        );
    }

    RenderComplete() {
        // TODO: Pass back information about the response.
        return (
            <div className="container-fluid">
                <h1>File Upload</h1>
                <p>File Uploaded</p>
            </div>
        );
    }

    SetForm() {
        return (
            <div className="container-fluid">
                <form onSubmit={e => this.handleSubmit(e)} noValidate>
                    <div className={'info-text'}>
                        <h1>Upload File</h1>
                        <Card>
                            <div className={'form-row'}>
                                {/*FirstName*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">First Name</label>
                                        <input type="text"
                                               name="firstName"
                                               className={'form-control user-input'}
                                               placeholder="firstName" value={this.state.form["firstName"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                    <div className="invalid-feedback d-block">
                                        {this.state.error}
                                    </div>
                                </div>
                                {/*Lastname*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Last Name</label>
                                        <input type="text"
                                               name="lastName"
                                               className={'form-control user-input'}
                                               placeholder="lastName" value={this.state.form["lastName"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                <div className="invalid-feedback d-block">
                                    {this.state.error}
                                </div>
                            </div>
                            {/*Address Demographics*/}
                            <div className={'form-row'}>

                            </div>
                            {/*Address Demographics*/}
                            <div className={'form-row'}>

                            </div>
                        </Card>

                        <Card>
                            <input type="file" onChange={e => this.setFile(e)}/>
                        </Card>
                        <button className="btn btn-primary" type="submit">Submit</button>
                    </div>

                </form>
            </div>
        )
    }


    render() {
        let contents = null;
        console.log(this.state)
        if (this.state.loading === false) {
            contents = this.SetForm()
        }
        if (this.state.loading === true) {
            contents = this.RenderLoading()
        }
        if (this.state.error === true) {
            throw new Error(this.state.errorMessage)
        }
        if (this.state.loading === false && this.state.complete === true) {
            contents = this.RenderComplete()
        }

        return (
            <div>
                {contents}
            </div>
        )
    }
}

export default FileUpload