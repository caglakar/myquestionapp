/** @jsxImportSource @emotion/react */
import { css } from '@emotion/react';
import { fontFamily, fontSize, gray1, gray2, gray5 } from './Styles';
import { UserIcon } from './Icon';
import { Link, useSearchParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { useAuth } from './Auth';

const buttonStyle = css`
  font-family: ${fontFamily};
  font-size: ${fontSize};
  padding: 5px 10px;
  background-color: transparent;
  color: ${gray2};
  text-decoration: none;
  cursor: pointer;
  :focus {
    outline-color: ${gray5};
  }
  span {
    margin-left: 7px;
  }
`;
type FormData = {
  search: string;
};
//Component defining with arrow function
const Header = () => {
  const navigate = useNavigate();
  const { register, handleSubmit } = useForm<FormData>();
  const [searchParams] = useSearchParams();
  const criteria = searchParams.get('criteria') || '';
  // const [search, setSearch] = useState(criteria);
  // const handleSearchInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  //   setSearch(e.currentTarget.value);
  // };
  // const handleSubmit = (e: React.FormEvent) => {
  //   e.preventDefault();
  //   //console.log(search);
  // };
  const submitForm = ({ search }: FormData) => {
    navigate(`search?criteria=${search}`);
  };

  const { isAuthenticated, user, loading } = useAuth();
  return (
    <div
      css={css`
        position: fixed;
        box-sizing: border-box;
        top: 0;
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 10px 20px;
        background-color: #fff;
        border-bottom: 1px solid ${gray5};
        box-shadow: 0 3px 7px 0 rgba(110, 112, 114, 0.21);
      `}
    >
      <Link
        to="/"
        css={css`
          font-size: 24px;
          font-weight: bold;
          color: ${gray1};
          text-decoration: none;
        `}
      >
        Q & A
        <span
          css={css`
            margin-left: 5px;
            font-size: 14px;
            font-weight: normal;
          `}
        >
          {process.env.REACT_APP_ENV || 'dev'}
        </span>
      </Link>
      {/* <a href="/">Q & A</a> */}
      <form onSubmit={handleSubmit(submitForm)}>
        <input
          //ref={register}
          {...register('search')}
          name="search"
          type="text"
          placeholder="Search..."
          defaultValue={criteria}
          css={css`
            box-sizing: border-box;
            font-family: ${fontFamily};
            font-size: ${fontSize};
            padding: 8px 10px;
            border: 1px solid ${gray5};
            border-radius: 3px;
            color: ${gray2};
            background-color: white;
            width: 200px;
            height: 30px;
            :focus {
              outline-color: ${gray5};
            }
          `}
        />
      </form>
      {/* <Link
        to="./signin"
        css={css`
          font-family: ${fontFamily};
          font-size: ${fontSize};
          padding: 5px 10px;
          background-color: transparent;
          color: ${gray2};
          text-decoration: none;
          cursor: pointer;
          :focus {
            outline-color: ${gray5};
          }
          span {
            margin-left: 7px;
          }
        `}
      >
        <UserIcon />
        <span>Sign In</span>
      </Link> */}

      <div>
        {!loading &&
          (isAuthenticated ? (
            <div>
              <span>{user!.name}</span>

              <Link to="signout" css={buttonStyle}>
                <UserIcon />
                <span>Sign Out</span>
              </Link>
            </div>
          ) : (
            <Link to="signin" css={buttonStyle}>
              <UserIcon />
              <span>Sign In</span>
            </Link>
          ))}
      </div>
    </div>
  );
};
export default Header;
