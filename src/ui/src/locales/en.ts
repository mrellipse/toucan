import { admin } from '../admin/locales/en';
export const en = {
    admin: admin,
    dict: {
        email: 'Email',
        errors: {
            unanticipated: 'An unanticipated error has occured'
        },
        enabled: 'Enabled',
        header: 'Header',
        heading: 'Heading',
        hello: 'Hello',
        label: 'Label',
        link: 'Link',
        name: 'Name',
        no: 'No',
        notImplemented: 'Not Implemented',
        of: 'of',
        project: 'Project',
        reports: 'Reports',
        role: 'Role',
        search: 'Search',
        secureContent: 'Displaying secure content for registered users only',
        settings: 'Settings',
        success: 'Success',
        update: 'Update',
        verified: 'Verified',
        yes: 'Yes'
    },
    forbidden: {
        title: 'Access Denied',
        content: 'Insufficent permissions'
    },
    home: {
        title: {
            begin: `My awesome`,
            end: 'project'
        },
        body: [
            `<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut dictum accumsan lorem in aliquam. Vestibulum ante
            ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Vivamus orci orci, cursus vulputate
            hendrerit eu, dapibus eu nisi. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos
            himenaeos. Nullam placerat dapibus tempor. Suspendisse nec mi at erat tincidunt mattis ut nec lacus. Praesent
            non nisl sed libero convallis tincidunt. Curabitur commodo aliquam vestibulum. Etiam rutrum risus quam, eget
            faucibus diam ornare in.</p>`,

            `<p>Morbi sollicitudin enim a rhoncus varius. Sed a lacus eu diam dapibus pellentesque sit amet at sem. Sed
            scelerisque pretium metus, sed mollis sem molestie eu. Suspendisse iaculis non erat ac consectetur. Phasellus
            placerat, eros ornare dictum efficitur, mauris dui varius risus, ac dignissim erat dolor lobortis augue.
            Praesent lectus augue, dignissim et sagittis in, convallis id nisl. Morbi interdum neque nec massa facilisis, ut
            rutrum justo semper. Phasellus fringilla auctor blandit. Maecenas nibh mi, cursus sed velit id, sodales semper
            sapien. Curabitur vitae nisl porta, tempus neque vitae, finibus nulla. Nam condimentum elit vel enim commodo, a
            tincidunt ante ornare. Duis libero sem, ultricies eu neque id, dapibus accumsan lectus.</p>`,

            `<p>Nam auctor urna at sapien porttitor lobortis. Morbi tincidunt vulputate posuere. Sed risus enim, vulputate
            pretium ultrices non, venenatis sed mauris. Nunc placerat metus ut odio varius aliquet. Proin diam lacus, congue
            in consequat eu, sagittis eget magna. Quisque eget diam tincidunt, laoreet ipsum nec, lobortis odio. Praesent
            non neque convallis, gravida nulla a, rutrum tortor. Class aptent taciti sociosqu ad litora torquent per conubia
            nostra, per inceptos himenaeos.</p>`
        ]
    },
    login: {
        title: 'Log In',
        instruction: 'Log in to your account.',
        invalid_token: 'Invalid token',
        secureRoute: 'Log in to continue',
        accessDenied: 'Insufficient permission',
        provider: {
            local: 'local',
            google: 'google',
            microsoft: 'microsoft'
        },
        placeholder: {
            email: 'Enter your email',
            password: 'Enter your password'
        },
        button: {
            submit: 'Access'
        }
    },
    navigation: {
        admin: 'Admin',
        dashboard: 'Dashboard',
        home: 'Home',
        login: 'Login',
        logout: 'Logout',
        profile: 'My Profile',
        signup: 'Sign Up'
    },
    notfound: {
        title: 'Page Not Found',
        instruction: `Unfortunate circumstances`
    },
    search: {
        title: 'Search Results for \'{0}\'',
        instruction: 'Not implemented'
    },
    signup: {
        title: 'Sign Up',
        instruction: 'Sign up for an account.',
        placeholder: {
            displayName: 'Enter your display name',
            userName: 'Enter your user name',
            password: 'Enter your password',
            confirmPassword: 'Confirm your password'
        },
        button: {
            submit: 'Submit'
        },
        registered: 'Already registered',
        complexity: 'Does not meet complexity requirements',
        password: 'password'
    },
    site: {
        copyright: '© Copyright 2058, Toucan Corporation'
    },
    user: {
        updated: 'User updated'
    },
    validation: {
        email: 'Must be a valid email address',
        required: 'Required',
        minLength: 'Must be at least {0} characters',
        sameAs: 'Must match {0}',
        login: {
            EmailAddressInUse: 'Email address is already being used',
            InvalidAccessToken: 'Invalid access token',
            InvalidNonce: 'Invalid nonce',
            FailedToResolveUser: 'Invalid username or password',
            FailedToVerifyUser: 'Failed to verify user',
            Network: 'Network error'
        }
    },
    verify: {
        title: 'Verification',
        sendCode: 'Send verification code',
        enterCode: 'Enter verification code',
        codeAck: 'Code acknowledged'
    }
};

export default en;