/** @jsxImportSource @emotion/react */
import { css } from '@emotion/react';
import Header from './Header';
import { HomePage } from './HomePage';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { SearchPage } from './Search';
import { SignIn } from './SignInPage';
import { NotFoundPage } from './NotFoundPage';
import { QuestionPage } from './QuestionPage';
import { fontFamily, fontSize, gray2 } from './Styles';
import React from 'react';
//import { configureStore } from './Store';
import { SignOut } from './SignOutPage';
import { AuthProvider } from './Auth';
import { AuthorizedPage } from './AuthorizedPage';

const AskPage = React.lazy(() => import('./AskPage'));

//const store = configureStore();

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <div
          css={css`
            font-family: ${fontFamily};
            font-size: ${fontSize};
            color: ${gray2};
          `}
        >
          <Header />
          <Routes>
            <Route path="" element={<HomePage />} />
            <Route path="search" element={<SearchPage />} />
            <Route
              path="ask"
              element={
                <React.Suspense fallback={<div>Loading...</div>}>
                  <AuthorizedPage>
                    <AskPage />
                  </AuthorizedPage>
                </React.Suspense>
              }
            />
            <Route path="signin" element={<SignIn action="signin" />} />
            <Route
              path="/signin-callback"
              element={<SignIn action="signin-callback" />}
            />

            <Route path="signout" element={<SignOut action="signout" />} />
            <Route
              path="/signout-callback"
              element={<SignOut action="signout-callback" />}
            />
            <Route path="questions/:questionId" element={<QuestionPage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
