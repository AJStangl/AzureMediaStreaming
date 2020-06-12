import React from 'react';
import {Card} from "reactstrap";
import Loader from "./Loader";

class VideoUpload extends React.Component {

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
                email: '',
                phoneNumber: '',
                street: '',
                zipCode: '',
                city: '',
                state: '',
                date: '',
                time: '',
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

        const data = new FormData(event.target);
        data.set('file', this.state.form.file);

        this.setState({
            loading: true
        })
        await fetch('/media/video', {
            method: 'POST',
            body: data,
        }).then(async response => {
            if (response.ok) {
                await response.json().then(x => {
                    this.setState({
                        complete: true,
                        loading: false
                    })
                })
                this.setState({
                    complete: true,
                    loading: false
                })
            } else {
                // Get the error message:

                await response.json().then(x => {
                    console.log(x.errorMessage);
                    this.setState({
                        complete: false,
                        loading: false,
                        error: true,
                        errorMessage: x.errorMessage
                    })
                }).catch(e => {
                    this.setState({
                        complete: false,
                        loading: false,
                        error: true,
                        errorMessage: response.statusText
                    })
                })
            }
        });
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
        return (
            <div className="container-fluid">
                <h1>File Upload</h1>
                <p>File Uploaded</p>
            </div>
        );
    }

    renderRow(name, display, type) {
        return <div className="col-md-6">
            <div className="form-group">
                <label className="font-weight-bold">{display}</label>
                <input type={type}
                       name={name}
                       className={'form-control user-input'}
                       placeholder={name} value={this.state.form[name]}
                       required={true}
                       onChange={this.handleChange.bind(this)}/>
            </div>
        </div>
    }

    renderStaticRow(name, value, display) {
        return (
            <div className="col-md-6">
                <div className="form-group">
                    <label className="font-weight-bold">{display}</label>
                    <input type="text"
                           name={name}
                           className={'form-control user-input'}
                           placeholder="city" value={value}
                           required={true}
                           readOnly={true}/>
                </div>
            </div>)
    }

    RenderHandledError() {
        return (
            <div>
                <h1>Something Went Wrong</h1>
                <h2>
                    {this.state.errorMessage}
                </h2>
            </div>
        );
    }

    SetForm() {
        return (
            <div className="container-fluid">
                <form onSubmit={e => this.handleSubmit(e)}>
                    <div className={'info-text'}>
                        <h1>Upload File</h1>
                        <Card>
                            <div className={'form-row'}>
                                {/*Contact Information*/}
                                {this.renderRow("firstName", "First Name", "text")}
                                {this.renderRow("lastName", "Last Name", "text")}
                                {this.renderRow("phoneNumber", "Phone Number", "text")}
                                {this.renderRow("email", "Email", "text")}
                            </div>
                            <div className={'form-row'}>
                                {this.renderRow("street", "Street Address", "text")}
                                {this.renderRow("zipCode", "Zip Code", "text")}
                                {this.renderStaticRow("city", "Baltimore", "City")}
                                {this.renderStaticRow("state", "Md", "State")}
                            </div>
                            <div className={'form-row'}>
                                {this.renderRow("date", "Date", "date")}
                                {this.renderRow("time", "Time", "time")}
                            </div>
                            <input type="file"
                                   onChange={e => this.setFile(e)}
                                   required={true}/>
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
        // console.log(this.state)
        if (this.state.loading === false) {
            contents = this.SetForm()
        }
        if (this.state.loading === true) {
            contents = this.RenderLoading()
        }
        if (this.state.error === true) {
            contents = this.RenderHandledError()
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

export default VideoUpload